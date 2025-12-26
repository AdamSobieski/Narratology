## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton : ITraversable
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<in TInput> : IAutomaton, ITraversable<TInput>
{

}

public interface IAutomaton<TState, TEdge, in TInput> : IAutomaton<TInput>, ITraversable<TState, TEdge, TInput>
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

public interface IAcceptor<in TInput> : IAutomaton<TInput>, IAcceptor
{
    public bool Accepts(IEnumerable<TInput> sequence);
}

public interface IAcceptor<TState, TEdge, in TInput> : IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>
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

public interface ITransducer<in TInput, out TOutput> : IAutomaton<TInput>, ITransducer, ITraversable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<TState, TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITraversable<TState, TEdge, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput>
{

}
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Automata Traversal and Reactive Programming

Resembling how collections can be enumerated with `IEnumerable` and `IEnumerator`, automata could be traversed. Interfaces for automata, and, thus, acceptors and transducers could provide a method, `GetTraverser()`, which returns objects for traversing them, objects implementing interfaces extending `IObserver<TInput>`, `IObservable<TOutput>`, and `ISubject<TInput, TOutput>` for interoperability with the `System.Reactive` library.

Here are some sketches of a set of `ITraversable` interfaces.

```cs
public interface ITraversable
{
    public ITraverser GetTraverser();
}

public interface ITraversable<in TInput> : ITraversable
{
    public new ITraverser<TInput> GetTraverser();
}

public interface ITraversable<in TInput, out TOutput> : ITraversable<TInput>
{
    public new ITraverser<TInput, TOutput> GetTraverser();
}

public interface ITraversable<TState, TEdge, in TInput> : ITraversable<TInput>
{
    public new ITraverser<TState, TEdge, TInput> GetTraverser();
}

public interface ITraversable<TState, TEdge, in TInput, out TOutput> :
    ITraversable<TInput, TOutput>, ITraversable<TState, TEdge, TInput>
{
    public new ITraverser<TState, TEdge, TInput, TOutput> GetTraverser();
}
```

Here are some sketches of automata traversers.

```cs
public interface IObserver
{
    void OnNext(object value);
    void OnError(Exception error);
    void OnCompleted();
}

public interface ITraverser : IObserver
{
    public IDisposable Subscribe(IObserver<(int Step, object Source, object Edge, object Target)> observer);
}

public interface ITraverser<in TInput> : ITraverser, IObserver<TInput> { }

public interface ITraverser<in TInput, out TOutput> : ITraverser<TInput>, ISubject<TInput, TOutput> { }

public interface ITraverser<TState, TEdge, in TInput> : ITraverser<TInput>
{
    public IDisposable Subscribe(IObserver<(int Step, TState Source, TEdge Edge, TState Target)> observer);
}

public interface ITraverser<TState, TEdge, in TInput, out TOutput> :
    ITraverser<TInput, TOutput>, ITraverser<TState, TEdge, TInput> { }
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
