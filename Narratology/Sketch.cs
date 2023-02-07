using AI.Agents;
using AI.Events;
using AI.Knowledge;
using AI.Planning;
using System.Collections;
using System.Collections.Trees;
using System.Linq.Expressions;

// 0.0.0.14

namespace System
{
    public interface ICloneable<out T> : ICloneable
        where T : ICloneable<T>
    {
        public new T Clone();
    }

    public interface IHasProperties
    {
        public IDictionary<string, object> Properties { get; }
    }

    public interface IHasMetadata
    {
        public IDictionary<string, object> Metadata { get; }
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

    public interface ISimilarity<in T>
    {
        public double Dissimilarity(T other);
    }

    public interface ISimilarityComparer<in T>
    {
        public double Dissimilarity(T x, T y);
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

namespace AI.Knowledge
{
    public interface IDelta
    {
        IEnumerable Add { get; }
        IEnumerable Remove { get; }
    }

    public interface IKnowledgebase : ICloneable<IKnowledgebase>
    {
        public bool Holds(object expr);
        public void Update(IDelta delta);
        public void Update(IEnumerable add, IEnumerable remove);
    }
}

namespace AI.Narratology
{
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
        public IEnumerable<IInterpretation> Responses { get; }
    }

    public interface IInterpretation : IThing
    {
        public IEvent Event { get; }

        public ISemantics Semantics { get; }
    }

    public interface ISemantics : IReadOnlyDictionary<IEventSequence, IThing>
    {
        public IThing this[params IEvent[] events]
        {
            get;
        }
    }

    public interface INarrative : IThing
    {
        public IFabula Fabula { get; }
        public IEnumerable<ISyuzhet> Syuzhets { get; }
    }

    public interface INarrator : IAgent
    {
        public IAsyncEnumerable<ISyuzhet> Create(IFabula fabula, IDictionary<string, object> args);
        public IAsyncEnumerable<INarration> Create(ISyuzhet syuzhet, IDictionary<string, object> args);
        public IAsyncEnumerable<IText> Create(INarration narration, IDictionary<string, object> args);

        //public IAsyncEnumerable<INarrative> Summarize(INarrative story, IDictionary<string, object> args);
    }

    public interface INarratee : IAgent
    {
        public IAsyncEnumerable<IInterpretation> Interpret(IText text, IDictionary<string, object> args);
    }
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