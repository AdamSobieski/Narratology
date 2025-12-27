## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<in TInput> : IAutomaton, ITraversable<TInput>
{

}

public interface IAutomaton<TState, TEdge, in TInput> : IAutomaton<TInput>, ITraversable<TEdge, TInput>
where TState : IHasOutgoingEdges<TEdge>
where TEdge : IHasTarget<TState>
{
    public new IEnumerable<TState> Start { get; }
}
```

Here are some sketches of interfaces for acceptors.

```cs
public interface IAcceptor : IAutomaton
{
    public bool Accepts(IEnumerable sequence);
}

public interface IAcceptor<in TInput> :
    IAutomaton<TInput>, IAcceptor, IAcceptorTraversable<TInput>
{
    public bool Accepts(IEnumerable<TInput> sequence);
}

public interface IAcceptor<TState, TEdge, in TInput> :
    IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>, IAcceptorTraversable<TEdge, TInput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>
{

}
```

Here are some sketches of interfaces for transducers.
```cs
public interface ITransducer : IAutomaton
{
    public IEnumerable Transduce(IEnumerable sequence);
}

public interface ITransducer<in TInput, out TOutput> :
    IAutomaton<TInput>, ITransducer, ITransducerTraversable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<TState, TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITransducerTraversable<TEdge, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput>
{

}
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Automata Traversal and Reactive Programming

Resembling how collections can be enumerated with `IEnumerable` and `IEnumerator`, automata could be traversed.

Interfaces for automata, and, thus, acceptors and transducers could provide a method, `GetTraverser()`, which returns objects for traversing them, objects implementing interfaces for interoperability with the `System.Reactive` library.

Here are some sketches of a set of `ITraversable`-related and `ITraverser`-related interfaces.

```cs
public interface ITraverser<in TInput> : IObserver<TInput, IEnumerable>, IObserver<TInput> { }
public interface ITraverser<TEdge, in TInput> : ITraverser<TInput>, IObserver<TInput, IEnumerable<TEdge>> { }

public interface ITraversable<in TInput>
{
    public ITraverser<TInput> GetTraverser();
}
public interface ITraversable<TEdge, in TInput> : ITraversable<TInput>
{
    public new ITraverser<TEdge, TInput> GetTraverser();
}



public interface IAcceptorTraverser<in TInput> : ITraverser<TInput>, ISubject<TInput, bool> { }
public interface IAcceptorTraverser<TEdge, in TInput> : ITraverser<TEdge, TInput>, IAcceptorTraverser<TInput> { }

public interface IAcceptorTraversable<in TInput> : ITraversable<TInput>
{
    public new IAcceptorTraverser<TInput> GetTraverser();
}
public interface IAcceptorTraversable<TEdge, in TInput> : ITraversable<TEdge, TInput>, IAcceptorTraversable<TInput>
{
    public new IAcceptorTraverser<TEdge, TInput> GetTraverser();
}



public interface ITransducerTraverser<in TInput, out TOutput> : ITraverser<TInput>, ISubject<TInput, TOutput> { }
public interface ITransducerTraverser<TEdge, in TInput, out TOutput> :
    ITraverser<TEdge, TInput>, ITransducerTraverser<TInput, TOutput> { }
    
public interface ITransducerTraversable<in TInput, out TOutput> : ITraversable<TInput>
{
    public new ITransducerTraverser<TInput, TOutput> GetTraverser();
}
public interface ITransducerTraversable<TEdge, in TInput, out TOutput> :
    ITraversable<TEdge, TInput>, ITransducerTraversable<TInput, TOutput>
{
    public new ITransducerTraverser<TEdge, TInput, TOutput> GetTraverser();
}
```

For developer convenience, default implementations of automata traversers can be provided.

## Learning, Building, and Optimizing Automata

_Coming soon._

## Tree Automata

Here are some sketches of interfaces for top-down and bottom-up tree acceptors.

### Top-down

```cs
public interface ITopDownTreeAcceptor
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public IEnumerable Start { get; }

    public bool Accepts(object tree);
}

public interface ITopDownTreeAcceptor<TTree> : ITopDownTreeAcceptor
    where TTree : IHasChildren<TTree>
{
    public bool Accepts(TTree tree);
}

public interface ITopDownTreeAcceptor<TRule, TState, TTree> : ITopDownTreeAcceptor<TTree>
    where TRule : ITopDownTreeAcceptorRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public new IReadOnlyDictionary<TState, IEnumerable<TRule>> Rules { get; }
    public new IEnumerable<TState> Start { get; }
}

public interface ITopDownTreeAcceptorRule<out TState, in TTree> : IMatcher<TTree>
{
    public TState Input { get; }
    public IReadOnlyList<TState> Output { get; }
}
```

### Bottom-up

```cs
public interface IBottomUpTreeAcceptor
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public Func<object, object> KeySelector { get; }

    public bool Accepts(object tree);
}

public interface IBottomUpTreeAcceptor<TTree> : IBottomUpTreeAcceptor
    where TTree : IHasChildren<TTree>
{
    public new Func<TTree, object> KeySelector { get; }

    public bool Accepts(TTree tree);
}


public interface IBottomUpTreeAcceptor<TRule, TState, TTree> : IBottomUpTreeAcceptor<TTree>
    where TRule : IBottomUpTreeAcceptorRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public new IReadOnlyDictionary<object, IEnumerable<TRule>> Rules { get; }
}

public interface IBottomUpTreeAcceptorRule<out TState, in TTree> : IMatcher<TTree>
{
    public IReadOnlyList<TState> Input { get; }
    public TState Output { get; }
}
```
