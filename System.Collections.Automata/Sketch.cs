namespace System
{
    public interface IResumeable<T>
    {
        public T Suspend();
        public void Resume(T state);
    }
}

namespace System.Collections.Automata
{

}

namespace System.Collections.Automata.Learning
{

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