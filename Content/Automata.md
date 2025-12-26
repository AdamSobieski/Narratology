## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<in TInput> : IAutomaton
{

}

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>
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

public interface IAcceptor<out TState, out TEdge, in TInput> : IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>
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

public interface ITransducer<in TInput, out TOutput> : IAutomaton<TInput>, ITransducer
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<out TState, out TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput>
{

}
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Building and Optimizing Automata

An automata builder could be provided which might, additionally, configurably, optimize described automata.

## Reactive Programming

Interfaces for acceptors and, in particular, transducers could provide a method, `GetTraverser()`, which returns an object for traversing the automata, one implementing interfaces extending `IObserver<TInput>`, `IObservable<TOutput>`, and `ISubject<TInput, TOutput>` for interoperability with the `System.Reactive` library.

Additionally, `IAcceptor<TState, TEdge, TInput>` and `ITransducer<TState, TEdge, TInput, TOutput>` could provide secondary traversers implementing interfaces extending something like `ISubject<TInput, (int Step, TState From, TEdge Edge, TState To)>`, providing observability with respect to state transitions occurring during traversals. The `int` step counter could be of use for grouping state transitions for non-deterministic implementations.

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
