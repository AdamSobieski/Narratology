using AI.Agents;
using AI.Epistemology;
using AI.Epistemology.Reasoning;
using AI.Events;
using AI.Narratology.Annotation;
using AI.Narratology.Stylistics;
using AI.Planning;
using System.Collections;
using System.Collections.Graphs;
using System.Collections.Trees;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Advanced;
using System.Linq.Expressions;

// 0.0.2.7

namespace System
{
    public interface ICloneable<out T> : ICloneable
        where T : ICloneable<T>
    {
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

    public interface INamed
    {
        public string Name { get; }
    }

    public interface INamespaceNamed : INamed
    {
        public string Namespace { get; }
        public string FullName { get; }
    }
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
        IEnumerable<T> Add { get; }
        IEnumerable<T> Remove { get; }
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
        void Add(string key, object? value, Type type);
        void Add(string key, IEnumerable<IConstraint> constraints);

        bool Contains(string key, Type type);
        bool TryGetType(string key, [MaybeNullWhen(false)] out Type type);

        bool TryGetValue(string key, [NotNullWhen(true)] out object? value, [NotNullWhen(true)] out IEnumerable justifications);
        bool TrySetValue(string key, object? value, IEnumerable justifications);
    }
}

namespace System.Collections.Graphs
{
    public interface IConnectionFrom<out TSource> : IThing
    {
        public TSource Source { get; }
    }

    public interface IConnectionTo<out TDestination> : IThing
    {
        public TDestination Destination { get; }
    }

    public interface IConnection<out TSource, out TDestination> : IConnectionFrom<TSource>, IConnectionTo<TDestination> { }

    public interface IConnection<out TSource, out TDestination, out TConnection> : IConnection<TSource, TDestination>
    {
        public TConnection Connection { get; }
    }
}

namespace System.Collections.Trees
{
    public interface ITreeNode<out TNode>
        where TNode : ITreeNode<TNode>
    {
        public IReadOnlyList<TNode> Children { get; }
    }

    public interface ITreeNodeParented<out TNode> : ITreeNode<TNode>
        where TNode : ITreeNodeParented<TNode>
    {
        public TNode? Parent { get; }
    }

    public interface ITreeNodeSiblinged<out TNode> : ITreeNodeParented<TNode>
        where TNode : ITreeNodeParented<TNode>
    {
        public TNode? PreviousSibling { get; }
        public TNode? NextSibling { get; }
    }
}

namespace System.Linq.Advanced
{
    public interface IInspectableMethod
    {
        public LambdaExpression Expression { get; }

        public object Invoke(object?[] args);
    }

    public interface IInspectableFunc<T, TResult> : IInspectableMethod
    {
        public new Expression<Func<T, TResult>> Expression { get; }

        public TResult Invoke(T arg);
    }

    public interface IInspectableAction<T> : IInspectableMethod
    {
        public new Expression<Action<T>> Expression { get; }

        public void Invoke(T arg);
    }

    public interface IEffect : IInspectableMethod
    {
        public new void Invoke(object?[] args);
    }

    public interface IEffect<T1> : IEffect
    {
        public new Expression<Action<T1>> Expression { get; }

        public void Invoke(T1 value);
    }

    public interface IProduction : IInspectableMethod { }

    public interface IProduction<TInput, TOutput> : IProduction
    {
        public new Expression<Func<TInput, TOutput>> Expression { get; }

        public TOutput Invoke(TInput input);
    }
}

namespace AI.Agents
{
    public interface IAgent : IThing
    {
        public IKnowledgebase Beliefs { get; }
        public IEnumerable Desires { get; }
        public IEnumerable Intentions { get; }
    }
}

namespace AI.Epistemology
{
    public sealed class Predicate : INamespaceNamed, IHasConstraints
    {
        public static IEnumerable<IConstraint> GenerateTypeConstraints(Type[] types)
        {
            int length = types.Length;
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<Expression> typeisexprs = new List<Expression>();
            for (int index = 0; index < length; ++index)
            {
                var p = Expression.Parameter(typeof(object));
                parameters.Add(p);
                typeisexprs.Add(Expression.TypeIs(p, types[index]));
            }
            for (int index = 0; index < length; ++index)
            {
                yield return new Constraint(Expression.Lambda(typeisexprs[index], parameters), $"Argument {index} was not of type {types[index].FullName}.");
            }
        }
        public static IEnumerable<IConstraint> GenerateTypeOrVariableConstraints(Type[] types)
        {
            int length = types.Length;
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<Expression> typeisexprs = new List<Expression>();
            var isassignableto = typeof(Type).GetMethod("IsAssignableTo");
            if (isassignableto == null) throw new Exception();

            for (int index = 0; index < length; ++index)
            {
                var p = Expression.Parameter(typeof(object));
                parameters.Add(p);
                typeisexprs.Add(
                    Expression.OrElse(
                        Expression.TypeIs(p, types[index]),
                        Expression.AndAlso(
                            Expression.TypeIs(p, typeof(ParameterExpression)),
                            Expression.Call(Expression.PropertyOrField(Expression.Convert(p, typeof(ParameterExpression)), "Type"), isassignableto, Expression.Constant(types[index]))
                            )
                        )
                    );
            }
            for (int index = 0; index < length; ++index)
            {
                yield return new Constraint(Expression.Lambda(typeisexprs[index], parameters), $"Argument {index} was not of type {types[index].FullName} or a variable of that type.");
            }
        }

        private class Constraint : IConstraint
        {
            public Constraint(LambdaExpression lambda, string text)
            {
                Expression = lambda;
                function = null;
                this.text = text;
            }

            public LambdaExpression Expression { get; }

            public string Name => Expression.Name ?? string.Empty;

            private Delegate? function;
            private readonly string text;

            public bool Invoke(object?[] args)
            {
                if (function == null)
                {
                    function = Expression.Compile();
                }
                return (bool)(function?.Method.Invoke(null, args) ?? false);
            }

            object IInspectableMethod.Invoke(object?[] args)
            {
                return Invoke(args);
            }

            public override string ToString()
            {
                return text;
            }
        }

        public Predicate(string @namespace, string name, int arity)
        {
            if (arity < 1) throw new ArgumentException();

            Namespace = @namespace;
            Name = name;
            Arity = arity;
            Constraints = Enumerable.Empty<IConstraint>();
        }
        public Predicate(string @namespace, string name, int arity, Type[] types)
        {
            if (arity < 1) throw new ArgumentException();
            if (arity != types.Length) throw new ArgumentException();

            Namespace = @namespace;
            Name = name;
            Arity = arity;
            Constraints = GenerateTypeOrVariableConstraints(types);
        }
        public Predicate(string @namespace, string name, int arity, IEnumerable<IConstraint> constraints)
        {
            if (arity < 1) throw new ArgumentException();

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

        public bool CanCreate(object?[] args, [NotNullWhen(false)] out AggregateException? reason)
        {
            List<Exception> reasons = new List<Exception>();
            foreach (var constraint in Constraints)
            {
                if (!constraint.Invoke(args))
                {
                    reasons.Add(new ConstraintNotSatisfiedException(constraint));
                }
            }
            if (reasons.Count > 0)
            {
                reason = new AggregateException(reasons);
                return false;
            }
            else
            {
                reason = null;
                return true;
            }
        }

        public Statement Create(params object?[] args)
        {
            if (CanCreate(args, out AggregateException? reason))
            {
                return new Statement(this, args);
            }
            else
            {
                throw reason;
            }
        }
    }

    public sealed class Statement : IInvariant
    {
        internal Statement(Predicate predicate, object?[] args)
        {
            Predicate = predicate;
            Arguments = args;
        }

        public Predicate Predicate { get; }
        public IReadOnlyList<object?> Arguments { get; }

        public bool IsValid
        {
            get
            {
                return Predicate.CanCreate(Arguments.ToArray(), out AggregateException? reason);
            }
        }
    }

    public interface IKnowledgebase : ICloneable<IKnowledgebase>, IInvariant, IQueryable<Statement>
    {
        public IConnection<IKnowledgebase, IKnowledgebase, IReasoner>? Binding { get; }

        public bool Contains(Statement expression);
        public bool Contains(Statement expression, out IEnumerable derivations);

        public bool IsReadOnly { get; }

        public void Update(IDelta<Statement> delta);
        public void Update(IEnumerable<Statement> add, IEnumerable<Statement> remove);
    }
}

namespace AI.Epistemology.Argumentation
{
    public interface IJustified<out T>
    {
        public T Value { get; }
        IEnumerable Justifications { get; }
    }
}

namespace AI.Epistemology.Reasoning
{
    public interface IConstraint : INamed, IInspectableMethod
    {
        public new bool Invoke(object?[] args);
    }

    public interface IConstraint<T> : IConstraint
    {
        public new Expression<Func<T, bool>> Expression { get; }

        public bool Invoke(T arg);
    }

    public interface IHasConstraints
    {
        public IEnumerable<IConstraint> Constraints { get; }
    }

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

    public interface IRule<TInput, TOutput> : INamed, IProduction<TInput, TOutput> { }

    public interface IReasoner
    {
        public ICollection<IConstraint<IQueryable<Statement>>> Constraints { get; }
        public ICollection<IRule<IQueryable<Statement>, IQueryable<Statement>>> Rules { get; }

        public IKnowledgebase Bind(IKnowledgebase source);
    }
}

namespace AI.Events
{
    public interface IEvent : IThing
    {
        public int? CompareStartToStart(IEvent other);
        public int? CompareStartToEnd(IEvent other);
        public int? CompareEndToStart(IEvent other);
        public int? CompareEndToEnd(IEvent other);
    }

    public interface IEventSequence : IReadOnlyList<IEvent>, IContainer<IEvent>, IEquatable<IEventSequence> { }

    public interface IEventSet : IContainer<IEvent>, ICountable<IEvent>, IEquatable<IEventSet>
    {
        public bool SubsetOf(IEventSet other);
        public bool SupersetOf(IEventSet other);
    }
}

namespace AI.Narratology
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

    public interface IInterpretation : IThing
    {
        public IEvent Event { get; }

        public ISemantics Semantics { get; }

        public IText Text { get; }
    }

    public interface ISemantics : IReadOnlyDictionary<IEventSequence, IThing>
    {
        public IThing this[params IEvent[] events]
        {
            get;
        }
    }

    public interface INarrator : IAgent
    {
        public IAsyncEnumerable<ISyuzhet> Create(IFabula fabula, IStyle style, IDictionary<string, object> args);
        public IAsyncEnumerable<INarration> Create(ISyuzhet syuzhet, IStyle style, IDictionary<string, object> args);
        public IAsyncEnumerable<IText> Create(INarration narration, IStyle style, IDictionary<string, object> args);

        //public IAsyncEnumerable<INarrative> Summarize(INarrative story, IStyle style, IDictionary<string, object> args);
    }

    public interface INarratee : IAgent
    {
        public IAsyncEnumerable<IInterpretation> Interpret(IText text, IDictionary<string, object> args);
    }
}

namespace AI.Narratology.Annotation
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

namespace AI.Narratology.Causality
{
    public interface ICausalReasoner : IThing
    {
        public bool? Caused(IEnumerable<IEvent> x, IEvent y, IDictionary<string, object> args);
        public bool? Caused(IEnumerable<IEvent> x, IEnumerable<IEvent> y, IDictionary<string, object> args)
        {
            foreach (var e in y)
            {
                bool? iterand = Caused(x, e, args);
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

namespace AI.Narratology.Coherence
{

}

namespace AI.Narratology.Drama
{
    public interface ICharacter : IAgent { }
}

namespace AI.Narratology.Hermeneutics
{

}

namespace AI.Narratology.Stylistics
{
    public interface IStyle : IThing
    {
        public IKnowledgebase Content { get; }

        public object? GetTechnique(Type techniqueType);
    }
}

namespace AI.Narratology.Thematics
{

}

namespace AI.Planning
{
    public interface IState : ICloneable<IState>, IInvariant
    {
        public IKnowledgebase Content { get; }
    }

    public interface IAction : ITreeNode<IAction>
    {
        public IEnumerable<IConstraint<IState>> Preconditions { get; }

        public IEnumerable<IEffect<IState>> Effects { get; }
    }

    public interface IPlan : IThing
    {

    }

    public interface IPlanner : IAgent
    {

    }
}