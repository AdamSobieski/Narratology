using AI.Agents;
using AI.Epistemology;
using AI.Events;
using AI.Narratology.Stylistics;
using AI.Planning;
using System.Collections;
using System.Collections.Trees;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

// 0.0.1.2

namespace System
{
    public interface ICloneable<out T> : ICloneable
        where T : ICloneable<T>
    {
        public new T Clone();
    }

    public interface IHasProperties
    {
        public ITypedStringDictionary Properties { get; }
    }

    public interface IHasMetadata
    {
        public ITypedStringDictionary Metadata { get; }
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

    public interface ITypedStringDictionary : IDictionary<string, object>
    {
        void Add(string key, object value, Type type);
        bool Contains(string key, Type type);
        bool TryGetType(string key, [MaybeNullWhen(false)] out Type type);
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
    public sealed class Predicate : IHasConstraints
    {
        public static IEnumerable<IConstraint> GenerateTypeConstraints(Type[] types)
        {
            int length = types.Length;
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<System.Linq.Expressions.Expression> typeisexprs = new List<System.Linq.Expressions.Expression>();
            for (int index = 0; index < length; ++index)
            {
                var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                parameters.Add(p);
                typeisexprs.Add(System.Linq.Expressions.Expression.TypeIs(p, types[index]));
            }
            for (int index = 0; index < length; ++index)
            {
                yield return new SimpleConstraint(System.Linq.Expressions.Expression.Lambda(typeisexprs[index], parameters), $"Argument {index} was not of type {types[index].FullName}.");
            }
        }

        private class SimpleConstraint : IConstraint
        {
            public SimpleConstraint(LambdaExpression lambda, string text)
            {
                Expression = lambda;
                function = null;
                this.text = text;
            }

            public LambdaExpression Expression { get; }
            private Delegate? function;
            private string text;

            public bool Satisfied(object[] args)
            {
                if (function == null)
                {
                    function = Expression.Compile();
                }
                return (bool)(function.Method.Invoke(null, args) ?? false);
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
            Constraints = GenerateTypeConstraints(types);
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
        public int Arity { get; }

        public IEnumerable<IConstraint> Constraints { get; }

        public bool CanCreate(object[] args, [NotNullWhen(false)] out AggregateException? reason)
        {
            List<Exception> reasons = new List<Exception>();
            foreach (var constraint in Constraints)
            {
                if (!constraint.Satisfied(args))
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

        public Expression Create(params object[] args)
        {
            if (CanCreate(args, out AggregateException? reason))
            {
                return new Expression(this, args);
            }
            else
            {
                throw reason;
            }
        }
    }

    public sealed class Expression
    {
        internal Expression(Predicate predicate, object[] args)
        {
            Predicate = predicate;
            Arguments = args;
        }

        public Predicate Predicate { get; }
        public IReadOnlyList<object> Arguments { get; }
    }

    public interface IKnowledgebase : ICloneable<IKnowledgebase>
    {
        public bool Holds(Expression expr);
        public void Update(IDelta<Expression> delta);
        public void Update(IEnumerable<Expression> add, IEnumerable<Expression> remove);
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
    }
}

namespace AI.Narratology.Thematics
{

}

namespace AI.Planning
{
    public interface IConstraint
    {
        public LambdaExpression Expression { get; }

        public bool Satisfied(object[] args);
    }

    public interface IConstraint<T1> : IConstraint
    {
        public new Expression<Func<T1, bool>> Expression { get; }

        public bool Satisfied(T1 arg);
    }

    public interface IConstraint<T1, T2> : IConstraint
    {
        public new Expression<Func<T1, T2, bool>> Expression { get; }

        public bool Satisfied(T1 arg1, T2 arg2);
    }

    public interface IConstraint<T1, T2, T3> : IConstraint
    {
        public new Expression<Func<T1, T2, T3, bool>> Expression { get; }

        public bool Satisfied(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IConstraint<T1, T2, T3, T4> : IConstraint
    {
        public new Expression<Func<T1, T2, T3, T4, bool>> Expression { get; }

        public bool Satisfied(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public sealed class ConstraintNotSatisfiedException : Exception
    {
        public ConstraintNotSatisfiedException(IConstraint constraint)
            : base(constraint.ToString())
        {
            Constraint = constraint;
        }

        public IConstraint Constraint { get; }
    }

    public interface IInvariant
    {
        public bool IsValid { get; }
    }

    public interface IEffect
    {
        public LambdaExpression Expression { get; }

        public void Process(object[] args);
    }

    public interface IEffect<T1> : IEffect
    {
        public new Expression<Action<T1>> Expression { get; }

        public void Process(T1 value);
    }

    public interface IHasConstraints
    {
        public IEnumerable<IConstraint> Constraints { get; }
    }

    public interface IHasPreconditions
    {
        public IEnumerable<IConstraint> Preconditions { get; }
    }

    public interface IHasEffects
    {
        public IEnumerable<IEffect> Effects { get; }
    }

    public interface IHasConstraints<T> : IHasConstraints
    {
        public new IEnumerable<IConstraint<T>> Constraints { get; }
    }

    public interface IHasPreconditions<T> : IHasPreconditions
    {
        public new IEnumerable<IConstraint<T>> Preconditions { get; }
    }

    public interface IHasEffects<T> : IHasEffects
    {
        public new IEnumerable<IEffect<T>> Effects { get; }
    }

    public interface IState : ICloneable<IState>, IInvariant
    {
        public IKnowledgebase Content { get; }
    }

    public interface IAction : ITreeNode<IAction>, IHasPreconditions<IState>, IHasEffects<IState> { }

    public interface IPlan : IThing
    {

    }

    public interface IPlanner : IAgent
    {

    }
}