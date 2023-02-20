namespace System
{
    public interface ICloneable<out T> : ICloneable
    where T : ICloneable<T>
    {
        object ICloneable.Clone() => Clone();

        public new T Clone();
    }

    public interface IResumeable<T>
    {
        public T Suspend();
        public void Resume(T state);
    }
}

namespace System.Collections.Automata
{
    // public interface IInspectableFunctionCollection<TInput> : IEnumerable<IInspectableFunc<TInput, bool>> { }
}

namespace System.Collections.Automata.Learning
{

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

    public interface IIndefiniteSimpleGraph<TNode>
    {
        public IEnumerable<TNode> GetAdjacentTargets(TNode node);

        public bool IsConnected(TNode source, TNode destination)
        {
            return GetAdjacentTargets(source).Any(n => object.Equals(source, destination));
        }
    }

    public interface IIndefiniteGraph<TNode, TEdge> : IIndefiniteSimpleGraph<TNode>
    {
        IEnumerable<TNode> IIndefiniteSimpleGraph<TNode>.GetAdjacentTargets(TNode node)
        {
            return GetOutgoingEdges(node).Select(GetDestination);
        }

        public IEnumerable<TEdge> GetEdgesBetween(TNode source, TNode destination)
        {
            return GetOutgoingEdges(source).Where(e => object.Equals(GetDestination(e), destination));
        }

        public IEnumerable<TEdge> GetOutgoingEdges(TNode node);

        public TNode GetDestination(TEdge edge);
    }

    public interface ISimpleGraph<TNode> : IIndefiniteSimpleGraph<TNode>, IEnumerable<TNode>
    {
        public int Count { get; }

        public IEnumerable<TNode> Nodes { get; }

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }
    }

    public interface IGraph<TNode, TEdge> : IIndefiniteGraph<TNode, TEdge>, ISimpleGraph<TNode>
    {

    }
}

namespace System.Collections.Trees
{
    public interface ITree<out TNode>
        where TNode : ITree<TNode>
    {
        public IReadOnlyList<TNode> Children { get; }
    }

    public interface ITreeParented<out TNode> : ITree<TNode>
        where TNode : ITreeParented<TNode>
    {
        public TNode? Parent { get; }
    }

    public interface ITreeSiblinged<out TNode> : ITreeParented<TNode>
        where TNode : ITreeParented<TNode>
    {
        public TNode? PreviousSibling { get; }
        public TNode? NextSibling { get; }
    }
}