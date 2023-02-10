using AI.Agents;
using AI.Epistemology;
using AI.Epistemology.Reasoning;
using AI.Events;
using AI.Narratology.Annotation;
using AI.Narratology.Hermeneutics;
using AI.Narratology.Stylistics;
using AI.Planning;
using System.Collections;
using System.Collections.Graphs;
using System.Collections.Trees;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

// 0.0.3.0

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

    public interface IInspectableFunc<T1, T2, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableFunc<T1, T2, T3, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInspectableAction<T> : IInspectableMethod
    {
        public new Expression<Action<T>> Expression { get; }

        public void Invoke(T arg);
    }

    public interface IInspectableAction<T1, T2> : IInspectableMethod
    {
        public new Expression<Action<T1, T2>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableAction<T1, T2, T3> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableAction<T1, T2, T3, T4> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
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
        void Define(string key, Type type);
        void Define(string key, IEnumerable<IConstraint> constraints);
        bool TryGetDefinition(string key, out IEnumerable<IConstraint> constraints);

        bool TryGetValue(string key, out object? value, [NotNullWhen(true)] out IEnumerable? justifications);
        bool TrySetValue(string key, object? value, IEnumerable justifications);
    }
}

namespace System.Collections.Graphs
{
    public interface ISource<out TSource>
    {
        public TSource Source { get; }
    }

    public interface IDestination<out TDestination>
    {
        public TDestination Destination { get; }
    }

    public interface IConnection<out TConnection>
    {
        public TConnection Connection { get; }
    }

    public interface IEdge<out TSource, out TDestination> : ISource<TSource>, IDestination<TDestination> { }

    public interface IEdge<out TSource, out TDestination, out TConnection> : IEdge<TSource, TDestination>, IConnection<TConnection> { }
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
    public sealed class Predicate : INamespaceNamed
    {
        public static IEnumerable<IConstraint> GenerateTypeConstraints(Type[] types)
        {
            int length = types.Length;
            List<ParameterExpression> parameters = new();
            List<Expression> typeisexprs = new();
            for (int index = 0; index < length; ++index)
            {
                var p = Expression.Parameter(typeof(object));
                parameters.Add(p);
                typeisexprs.Add(Expression.TypeIs(p, types[index]));
            }
            for (int index = 0; index < length; ++index)
            {
                yield return new Constraint(Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty, $"Argument {index} was not of type {types[index].FullName}.");
            }
        }
        public static IEnumerable<IConstraint> GenerateTypeOrVariableConstraints(Type[] types)
        {
            int length = types.Length;
            List<ParameterExpression> parameters = new();
            List<Expression> typeisexprs = new();
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
                yield return new Constraint(Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty, $"Argument {index} was not of type {types[index].FullName} or a variable of that type.");
            }
        }

        private class Constraint : IConstraint
        {
            public Constraint(LambdaExpression lambda, string name, string message)
            {
                Expression = lambda;
                function = null;
                m_name = name;
                m_message = message;
            }

            public LambdaExpression Expression { get; }
            private Delegate? function;
            private readonly string m_name;
            private readonly string m_message;

            public string Name => m_name;
            public string Message => m_message;

            public bool Invoke(object?[] args)
            {
                function ??= Expression.Compile();
                return (bool)(function.Method.Invoke(null, args) ?? false);
            }

            object IInspectableMethod.Invoke(object?[] args)
            {
                return Invoke(args);
            }
        }

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
            Constraints = GenerateTypeOrVariableConstraints(types);
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

        public bool CanInvoke(object?[] args, [NotNullWhen(false)] out Exception? reason)
        {
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

        public Statement Invoke(params object?[] args)
        {
            if (CanInvoke(args, out Exception? reason))
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
                return Predicate.CanInvoke(Arguments.ToArray(), out _);
            }
        }
    }

    public interface IKnowledgebase : ICloneable<IKnowledgebase>, IInvariant, IQueryable<Statement>
    {
        public IEdge<IKnowledgebase, IKnowledgebase, IReasoner>? Binding { get; }

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
        public IEnumerable Justifications { get; }
    }
}

namespace AI.Epistemology.Reasoning
{
    public interface IConstraint : INamed, IInspectableMethod
    {
        public string Message { get; }

        public new bool Invoke(object?[] args);
    }

    public interface IConstraint<T> : IConstraint, IInspectableFunc<T, bool> { }

    public sealed class ConstraintNotSatisfiedException : Exception
    {
        public ConstraintNotSatisfiedException(IConstraint constraint)
            : base(constraint.Message)
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

    public interface IRule<TInput, TOutput> : INamed, IInspectableFunc<TInput, TOutput> { }

    public interface IReasoner
    {
        public IEnumerable<IConstraint<IQueryable<Statement>>> Constraints { get; }
        public IEnumerable<IRule<IQueryable<Statement>, IQueryable<Statement>>> Rules { get; }

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

    public interface INarrator : IAgent
    {
        public IAsyncEnumerable<ISyuzhet> Create(IFabula fabula, IStyle style, IDictionary<string, object> args);
        public IAsyncEnumerable<INarration> Create(ISyuzhet syuzhet, IStyle style, IDictionary<string, object> args);
        public IAsyncEnumerable<IText> Create(INarration narration, IStyle style, IDictionary<string, object> args);
    }

    public interface INarratee : IAgent
    {
        public IAsyncEnumerable<IInterpretation> Interpret(IText text, IDictionary<string, object> args);
    }
}

namespace AI.Narratology.Aesthetics
{

}

namespace AI.Narratology.Aesthetics.Morality
{

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
}

namespace AI.Narratology.Hermeneutics.Semiotics
{

}

namespace AI.Narratology.Hermeneutics.Thematics
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

namespace AI.Planning
{
    public interface IState : ICloneable<IState>, IInvariant
    {
        public IKnowledgebase Content { get; }
    }

    public interface IDomain
    {
        public IEnumerable<IOperator> Operators { get; }
    }

    public interface IOperator : INamed
    {
        public IEnumerable Parameters { get; }

        public IAction Invoke(object?[] args);
    }

    public interface IAction : ITreeNode<IAction>, INamed
    {
        public IOperator Operator { get; }

        public IReadOnlyList<object?> Arguments { get; }

        public IEnumerable<IConstraint<IState>> Preconditions { get; }

        public IEnumerable<IInspectableAction<IState>> Effects { get; }
    }

    public interface IPlan : IThing
    {

    }

    public interface IPlanner : IAgent
    {

    }
}