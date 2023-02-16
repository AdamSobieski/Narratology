﻿using AI.Agents;
using AI.AutomatedPlanning;
using AI.Epistemology;
using AI.Epistemology.Adaptation;
using AI.Epistemology.Constraints;
using AI.Epistemology.Reasoning;
using AI.Narratology.Annotation;
using AI.Narratology.Events;
using AI.Narratology.Hermeneutics;
using AI.Narratology.Pragmatics;
using AI.Narratology.Stylistics;
using System.Collections;
using System.Collections.Graphs;
using System.Collections.Trees;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

// 0.0.4.42

namespace System
{
    public interface INamed
    {
        public string Name { get; }
    }

    public interface INamespaceNamed : INamed
    {
        public string Namespace { get; }
        public string FullName { get; }
    }

    public interface ICloneable<out T> : ICloneable
        where T : ICloneable<T>
    {
        object ICloneable.Clone() => Clone();

        public new T Clone();
    }

    public interface IHasProperties
    {
        public IDataDictionary Properties { get; }
    }

    public interface IHasMetadata
    {
        public IDataDictionary Metadata { get; }
    }

    public interface IThing : IHasProperties, IHasMetadata { }
}

namespace System.Collections.Generic
{
    public interface IContainer<in T>
    {
        public bool Contains(T element);
    }

    public interface ICountable<out T> : IEnumerable<T>
    {
        public int Count { get; }
    }

    public interface IDelta<out T>
    {
        IEnumerable<T> Additions { get; }
        IEnumerable<T> Removals { get; }
    }

    public interface ISimilarity<in T>
    {
        public double Dissimilarity(T other);
    }

    public interface ISimilarityComparer<in T>
    {
        public double Dissimilarity(T x, T y);
    }

    public interface IDataDictionary : IDictionary<string, object?>
    {
        void Define(string key, Type type);
        void Define(string key, IEnumerable<IConstraint> constraints);
        bool TryGetDefinition(string key, out IEnumerable<IConstraint> constraints);

        bool TryGetValue(string key, out object? value, [NotNullWhen(true)] out IEnumerable? justifications);
        bool TrySetValue(string key, object? value, IEnumerable justifications);
    }
}

namespace AI
{
    namespace Agents
    {
        public interface IAgent : IThing
        {
            public IStatementCollection Beliefs { get; }
            public IEnumerable Desires { get; }
            public IEnumerable Intentions { get; }
        }
    }

    namespace Epistemology
    {
        internal interface ITerm
        {
            public bool Unify(object? other, IDictionary<Variable, object?> substitutions);
            public bool Contains(Variable variable, IDictionary<Variable, object?> substitutions);
            public bool Replace(IDictionary<Variable, object?> substitutions, out object? value);
        }

        public sealed class Variable : ITerm
        {
            public Variable()
            {
                Constraints = Array.Empty<IConstraint<object?>>();
            }
            public Variable(string name)
            {
                Constraints = Array.Empty<IConstraint<object?>>();
                m_name = name;
            }
            public Variable(IEnumerable<IConstraint<object?>> constraints)
            {
                Constraints = constraints;
            }
            public Variable(IEnumerable<IConstraint<object?>> constraints, string name)
            {
                Constraints = constraints;
                m_name = name;
            }

            public IEnumerable<IConstraint<object?>> Constraints { get; }
            private string? m_name;

            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (!Constraints.All(constraint => constraint.Invoke(other))) return false;

                if (other is ITerm term)
                {
                    if (object.ReferenceEquals(this, term))
                    {
                        return true;
                    }

                    if (substitutions.TryGetValue(this, out object? value))
                    {
                        if (value is ITerm otherTerm)
                        {
                            return otherTerm.Unify(term, substitutions);
                        }
                    }

                    if (term.Contains(this, substitutions))
                    {
                        return false;
                    }

                    substitutions.Add(this, term);
                    return true;
                }
                else
                {
                    if (substitutions.TryGetValue(this, out object? value))
                    {
                        if (value is ITerm otherTerm)
                        {
                            return otherTerm.Unify(other, substitutions);
                        }
                        else
                        {
                            return object.Equals(other, value);
                        }
                    }
                    else
                    {
                        substitutions.Add(this, other);
                        return true;
                    }
                }
            }
            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                if (object.ReferenceEquals(this, variable)) return true;

                if (substitutions.TryGetValue(this, out object? value))
                {
                    if (value is ITerm otherTerm)
                    {
                        return otherTerm.Contains(variable, substitutions);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            bool ITerm.Replace(IDictionary<Variable, object?> substitutions, out object? value)
            {
                return substitutions.TryGetValue(this, out value);
            }

            public Statement Invoke(params object?[] args)
            {
                return new Statement(this, args);
            }

            public override string? ToString()
            {
                return m_name ?? base.ToString();
            }
        }

        public sealed class Predicate : INamespaceNamed, ITerm
        {
            public Predicate(string @namespace, string name, int arity)
            {
                if (arity < 1) throw new ArgumentException("Arity is less than 1.", nameof(arity));

                Namespace = @namespace;
                Name = name;
                Arity = arity;
                Constraints = Enumerable.Empty<IConstraint>();
            }
            public Predicate(string @namespace, string name, int arity, Type[] types)
            {
                if (arity < 1) throw new ArgumentException("Arity is less than 1.", nameof(arity));
                if (arity != types.Length) throw new ArgumentException("Number of types provided should equal arity.", nameof(types));

                Namespace = @namespace;
                Name = name;
                Arity = arity;
                Constraints = Constraint.GenerateTypeOrVariableConstraints(types);
            }
            public Predicate(string @namespace, string name, int arity, IEnumerable<IConstraint> constraints)
            {
                if (arity < 1) throw new ArgumentException("Arity is less than 1.", nameof(arity));

                Namespace = @namespace;
                Name = name;
                Arity = arity;
                Constraints = constraints;
            }

            public string Namespace { get; }
            public string Name { get; }
            public string FullName { get { return Namespace + "." + Name; } }

            public int Arity { get; }

            public IEnumerable<IConstraint> Constraints { get; }

            public bool CanInvoke(object?[]? args, [NotNullWhen(false)] out Exception? reason)
            {
                try
                {
                    if (args == null) throw new ArgumentNullException(nameof(args));
                    if (args.Length != Arity) throw new ArgumentException($"Expected {Arity} arguments in array.", nameof(args));

                    foreach (var constraint in Constraints)
                    {
                        if (!constraint.Invoke(args))
                        {
                            reason = new ConstraintNotSatisfiedException(constraint);
                            return false;
                        }
                    }
                    reason = null;
                    return true;
                }
                catch (Exception error)
                {
                    reason = error;
                    return false;
                }
            }
            public Statement Invoke(params object?[]? args)
            {
                if (CanInvoke(args, out Exception? reason))
                {
                    return new Statement(this, args ?? Array.Empty<object>());
                }
                else
                {
                    throw reason;
                }
            }

            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                return false;
            }
            bool ITerm.Replace(IDictionary<Variable, object?> substitutions, out object? value)
            {
                value = null;
                return false;
            }
            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (other is Variable v)
                {
                    return ((ITerm)v).Unify(this, substitutions);
                }
                else
                {
                    return object.Equals(this, other);
                }
            }
        }

        public sealed class Statement : ITerm
        {
            static Predicate s_and = new Predicate("AI.Epistemology", "And", 2);
            static Predicate s_or = new Predicate("AI.Epistemology", "Or", 2);
            static Predicate s_not = new Predicate("AI.Epistemology", "Not", 1);

            public static Statement And(object? x, object? y)
            {
                return s_and.Invoke(x, y);
            }
            public static Statement Or(object? x, object? y)
            {
                return s_or.Invoke(x, y);
            }
            public static Statement Not(object? x)
            {
                return s_not.Invoke(x);
            }

            internal Statement(Variable predicate, object?[] args)
            {
                Predicate = predicate;
                Arguments = args;
            }
            internal Statement(Predicate predicate, object?[] args)
            {
                Predicate = predicate;
                Arguments = args;
            }

            public object Predicate { get; }
            public IReadOnlyList<object?> Arguments { get; }

            public bool IsGround
            {
                get
                {
                    if (Predicate is Variable) return false;
                    int count = Arguments.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        var a = Arguments[index];

                        if (a is Statement s)
                        {
                            if (!s.IsGround) return false;
                        }
                        else if (a is Variable v)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public bool Matches(Statement other)
            {
                return ((ITerm)this).Unify(other, new Dictionary<Variable, object?>(0));
            }

            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                throw new NotImplementedException();
            }
            bool ITerm.Replace(IDictionary<Variable, object?> substitutions, out object? value)
            {
                throw new NotImplementedException();
            }
            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (other == null) return false;

                if (other is Variable v)
                {
                    return ((ITerm)v).Unify(this, substitutions);
                }
                else if (other is Statement s)
                {
                    int count = this.Arguments.Count;
                    if (count != s.Arguments.Count) return false;

                    if (!((ITerm)Predicate).Unify(s.Predicate, substitutions)) return false;
                    for (int index = 0; index < count; ++index)
                    {
                        var a = this.Arguments[index];
                        var o = s.Arguments[index];

                        if (a is ITerm term)
                        {
                            if (!term.Unify(o, substitutions)) return false;
                        }
                        else if (o is ITerm otherTerm)
                        {
                            if (!otherTerm.Unify(a, substitutions)) return false;
                        }
                        else
                        {
                            if (!object.Equals(a, o)) return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override string ToString()
            {
                return ((dynamic)Predicate).Name + "(" + string.Join(", ", Arguments.Select(a => a?.ToString())) + ")";
            }
        }

        public interface IStatementCollection : IQueryable<Statement>, IInvariant, ICloneable<IStatementCollection>
        {
            public interface IQueryResult : IEnumerable<Statement>
            {
                object? this[Variable key] { get; }

                IEnumerable<Variable> Keys { get; }
                IEnumerable<object?> Values { get; }

                bool ContainsKey(Variable key);
                bool TryGetValue(Variable key, [MaybeNullWhen(false)] out object? value);
            }

            public IEdge<IStatementCollection, IStatementCollection, IReasoner>? Binding { get; }

            public bool Contains(Statement expression);
            public bool Contains(Statement expression, out IEnumerable derivations);

            public IEnumerable<IQueryResult> Query(IEnumerable<Statement> query);

            public bool IsReadOnly { get; }

            public void Update(IDelta<Statement> delta)
            {
                Update(delta.Removals, delta.Additions);
            }
            public void Update(IEnumerable<Statement> removals, IEnumerable<Statement> additions);
        }
    }

    namespace Epistemology.Adaptation
    {
        public interface IAlternatives<T> : IEnumerable<(T Value, double Weight)>
        {
            public bool IsReadOnly { get; }

            public void Add((T Value, double Weight) alternative);
            public void Remove((T Value, double Weight) alternative);

            public bool SupportsFeedback { get; }

            public void Increase(T value, double intensity = 0.5d);
            public void Decrease(T value, double intensity = 0.5d);

            public void Select(T value, double intensity = 0.5d);
            public void Select(IEnumerable<T> values, double intensity = 0.5d);
        }

        public interface ILookup<TKey, TValue, TState>
        {
            public IAlternatives<TValue> Lookup((TKey Key, double Weight) input, (TState State, double Weight) context);

            public IAlternatives<TValue> Combine(IEnumerable<IAlternatives<TValue>> alternatives);
        }
    }

    namespace Epistemology.Argumentation
    {
        public interface IJustifiedValue<out T>
        {
            public T Value { get; }
            public IEnumerable Justifications { get; }
        }

        public interface IJustifiedComparable<in T> : IComparable<T>
        {
            public new IJustifiedValue<int> CompareTo(T? other);
        }
    }

    namespace Epistemology.Constraints
    {
        internal class Constraint : IConstraint
        {
            public static IEnumerable<IConstraint> GenerateTypeConstraints(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();
                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(System.Linq.Expressions.Expression.TypeIs(p, types[index]));
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint(System.Linq.Expressions.Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }
            public static IEnumerable<IConstraint> GenerateTypeOrVariableConstraints(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();

                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(
                        System.Linq.Expressions.Expression.OrElse(
                            System.Linq.Expressions.Expression.TypeIs(p, types[index]),
                            System.Linq.Expressions.Expression.TypeIs(p, typeof(Variable))
                            )
                        );
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint(System.Linq.Expressions.Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }
            public static IEnumerable<IConstraint<T>> GenerateTypeOrVariableConstraints<T>(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();

                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(
                        System.Linq.Expressions.Expression.OrElse(
                            System.Linq.Expressions.Expression.TypeIs(p, types[index]),
                            System.Linq.Expressions.Expression.TypeIs(p, typeof(Variable))
                            )
                        );
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint<T>(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }


            public Constraint(LambdaExpression lambda, string name)
            {
                Expression = lambda;
                m_delegate = null;
                m_name = name;
            }

            public LambdaExpression Expression { get; }
            private Delegate? m_delegate;
            private readonly string m_name;

            public string Name => m_name;

            public bool Invoke(object?[]? args)
            {
                if (args == null) throw new ArgumentNullException(nameof(args));
                m_delegate ??= Expression.Compile();
                return (bool)(m_delegate.DynamicInvoke(args) ?? false);
            }

            object? IInspectableDelegate.Invoke(object?[]? args)
            {
                return Invoke(args);
            }
        }

        internal class Constraint<T> : IConstraint<T>
        {
            public Constraint(Expression<Func<T, bool>> lambda, string name)
            {
                Expression = lambda;
                m_delegate = null;
                m_name = name;
            }

            public Expression<Func<T, bool>> Expression { get; }
            private Delegate? m_delegate;
            private readonly string m_name;

            public string Name => m_name;

            LambdaExpression IInspectableDelegate.Expression => Expression;

            public bool Invoke(object?[]? args)
            {
                if (args == null) throw new ArgumentNullException(nameof(args));
                m_delegate ??= Expression.Compile();
                return (bool)(m_delegate.DynamicInvoke(args) ?? false);
            }

            object? IInspectableDelegate.Invoke(object?[]? args)
            {
                return Invoke(args);
            }

            public bool Invoke(T arg)
            {
                m_delegate ??= Expression.Compile();
                return ((Func<T, bool>)m_delegate!)(arg);
            }
        }

        public interface IConstraint : INamed, IInspectableDelegate
        {
            public new bool Invoke(object?[]? args);
        }

        public interface IConstraint<T> : IConstraint, IInspectableFunc<T, bool> { }

        public interface IConstraint<T1, T2> : IConstraint, IInspectableFunc<T1, T2, bool> { }

        public interface IConstraint<T1, T2, T3> : IConstraint, IInspectableFunc<T1, T2, T3, bool> { }

        public interface IConstraint<T1, T2, T3, T4> : IConstraint, IInspectableFunc<T1, T2, T3, T4, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6, T7> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6, T7, T8> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, T8, bool> { }

        public sealed class ConstraintNotSatisfiedException : Exception
        {
            public ConstraintNotSatisfiedException(IConstraint constraint)
                : base($"Constraint {constraint.Name} not satisfied.")
            {
                Constraint = constraint;
            }

            public ConstraintNotSatisfiedException(IConstraint constraint, string message)
                : base(message)
            {
                Constraint = constraint;
            }

            public IConstraint Constraint { get; }
        }

        public interface IInvariant
        {
            public bool IsValid { get; }
        }

        public static class Optimization
        {
            public static Delegate Compile(this IEnumerable<IConstraint> constraints)
            {
                throw new NotImplementedException();
            }
            public static Func<T, bool> Compile<T>(this IEnumerable<IConstraint<T>> constraints)
            {
                throw new NotImplementedException();
            }
            public static Delegate CompileAssertion(this IEnumerable<IConstraint> constraints)
            {
                throw new NotImplementedException();
            }
            public static Action<T> CompileAssertion<T>(this IEnumerable<IConstraint<T>> constraints)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace Epistemology.Reasoning
    {
        public interface IRule<TInput, TOutput> : INamed, IInspectableFunc<TInput, TOutput> { }

        public interface IReasoner
        {
            public IEnumerable<IConstraint<IQueryable<Statement>>> Constraints { get; }
            public IEnumerable<IRule<IQueryable<Statement>, IQueryable<Statement>>> Rules { get; }

            public IStatementCollection Bind(IStatementCollection source);
        }
    }

    namespace Narratology
    {
        public interface INarrative : IThing
        {
            public IFabula Fabula { get; }
            public IEnumerable<ISyuzhet> Syuzhets { get; }
        }

        public interface IFabula : IThing
        {
            public IEventSet Events { get; }
        }

        public interface ISyuzhet : IThing
        {
            public IEventSequence Events { get; }

            public IEnumerable<INarration> Narrations { get; }
        }

        public interface INarration : IThing
        {
            public IEvent Event { get; }

            public IPlan Plan { get; }

            public IEnumerable<IInterpretation> IntendedInterpretations { get; }

            public IEnumerable<IText> Realizations { get; }
        }

        public interface IText : IThing
        {
            public ISegment CreateSegment(ISelection selection);
        }

        public interface INarrator : IAgent
        {
            public IAsyncEnumerable<ISyuzhet> Create(IFabula fabula, IStyle style);
            public IAsyncEnumerable<INarration> Create(ISyuzhet syuzhet, IStyle style);
            public IAsyncEnumerable<IText> Create(INarration narration, IStyle style);
        }

        public interface INarratee : IAgent
        {
            public IAsyncEnumerable<IInterpretation> Interpret(IText text);
        }
    }

    namespace Narratology.Aesthetics
    {

    }

    namespace Narratology.Aesthetics.Morality
    {

    }

    namespace Narratology.Annotation
    {
        public interface ISegment : IThing
        {
            public IText Text { get; }

            public ISelection Selection { get; }
        }

        public interface ISelection : ITreeNodeParented<ISelection>
        {
            public IText Text { get; }

            public int CompareStartToStart(ISelection other);
            public int CompareStartToEnd(ISelection other);
            public int CompareEndToStart(ISelection other);
            public int CompareEndToEnd(ISelection other);
        }
    }

    namespace Narratology.Causality
    {
        public interface ICausalReasoner : IThing
        {
            public bool? Caused(IEnumerable<IEvent> x, IEvent y);
            public bool? Caused(IEnumerable<IEvent> x, IEnumerable<IEvent> y)
            {
                foreach (var e in y)
                {
                    bool? iterand = Caused(x, e);
                    if (!iterand.HasValue)
                    {
                        return null;
                    }
                    else if (!iterand.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public interface IPlot : IThing
        {
            public IEventSequence Events { get; }
        }
    }

    namespace Narratology.Characters
    {
        public interface ICharacter : IAgent { }

        public interface ITrait { }

        public interface IModel
        {
            public double Probability(IEvent behavior, (IState State, double Weight) context);
        }

        public interface ITraitBasedModel : IModel
        {
            public IAlternatives<ITrait> Traits((IState State, double Weight) context);
        }
    }

    namespace Narratology.Coherence
    {

    }

    namespace Narratology.Completion
    {
        public interface IPrediction
        {
            public IAlternatives<IEnumerable<IEvent>> Predict(IEnumerable<IEvent> events /*, (IScope Scope, double Weight) context */);
        }

        public interface IInfilling
        {
            public IAlternatives<IEnumerable<IEvent>> Infill(IEnumerable<IEvent> before, IEnumerable<IEvent> after /*, (IScope Scope, double Weight) context */);
        }

        public interface IScript : IPrediction, IInfilling
        {
            // https://en.wikipedia.org/wiki/Behavioral_script
            // https://en.wikipedia.org/wiki/Schema_(psychology)

            public IAlternatives<bool> Contains(IEnumerable<IEvent> events);
        }

        public interface IScriptCollection : ILookup<IEnumerable<IEvent>, IScript, IState>
        {
            public IAlternatives<IEnumerable<IEvent>> Combine(IEnumerable<IAlternatives<IEnumerable<IEvent>>> alternatives);
        }
    }

    namespace Narratology.Drama
    {
        public interface IConflict
        {
            // https://en.wikipedia.org/wiki/Conflict_(process)
        }
    }

    namespace Narratology.Events
    {
        public interface IEvent : IThing
        {
            // https://plato.stanford.edu/entries/events/
            // https://en.wikipedia.org/wiki/Thematic_relation

            public int? CompareStartToStart(IEvent other);
            public int? CompareStartToEnd(IEvent other);
            public int? CompareEndToStart(IEvent other);
            public int? CompareEndToEnd(IEvent other);
        }

        public interface IEventSequence : IReadOnlyList<IEvent>, IContainer<IEvent>, IEquatable<IEventSequence> { }

        public interface IEventSet : IContainer<IEvent>, ICountable<IEvent>, IEquatable<IEventSet>
        {
            IEventSet ExceptWith(IEventSet other);
            IEventSet IntersectWith(IEventSet other);
            bool IsProperSubsetOf(IEventSet other);
            bool IsProperSupersetOf(IEventSet other);
            bool IsSubsetOf(IEventSet other);
            bool IsSupersetOf(IEventSet other);
            bool Overlaps(IEventSet other);
            bool SetEquals(IEventSet other);
            IEventSet SymmetricExceptWith(IEventSet other);
            IEventSet UnionWith(IEventSet other);
        }
    }

    namespace Narratology.Hermeneutics
    {
        public interface IInterpretation : IThing
        {
            public IEvent Event { get; }

            public IContent Content { get; }

            public IText Text { get; }
        }

        public interface IContent : IReadOnlyDictionary<IEventSequence, IThing>
        {
            public IThing this[params IEvent[] events]
            {
                get;
            }

            public IEnumerable<IEventSequence> KeysStartingWith(IEvent @event)
            {
                return Keys.Where(k => k.Count > 0 && @event.Equals(k[0]));
            }
            public IEnumerable<IEventSequence> KeysEndingWith(IEvent @event)
            {
                return Keys.Where(k => k.Count > 0 && @event.Equals(k[k.Count - 1]));
            }
            public IEnumerable<IEventSequence> KeysContaining(IEvent @event)
            {
                return Keys.Where(k => k.Contains(@event));
            }
        }
    }

    namespace Narratology.Hermeneutics.Semiotics
    {

    }

    namespace Narratology.Hermeneutics.Thematics
    {

    }

    namespace Narratology.Pragmatics
    {
        public interface IState : /*ICloneable<IState>,*/ IInvariant
        {
            public IStatementCollection Content { get; }

            public bool TryGetNext(IDelta<Statement> delta, out IState? state)
            {
                return TryGetNext(delta.Removals, delta.Additions, out state);
            }
            public bool TryGetNext(IEnumerable<Statement> removals, IEnumerable<Statement> additions, out IState? state);
        }
    }

    namespace Narratology.Stylistics
    {
        public interface IStyle : IThing
        {
            public IStatementCollection Content { get; }

            public object? GetTechnique(Type techniqueType);
        }
    }
}