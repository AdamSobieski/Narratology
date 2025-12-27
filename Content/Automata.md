## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton<in TInput> : INavigable<TInput>
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<TState, TEdge, in TInput> : IAutomaton<TInput>, INavigable<TState, TEdge, TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>
{
    public new IEnumerable<TState> Start { get; }
}
```

Here are some sketches of interfaces for acceptors.

```cs
public interface IAcceptor<in TInput> : IAutomaton<TInput>, IAcceptorNavigable<TInput>
{
    public bool Accepts(IEnumerable<TInput> sequence);
}

public interface IAcceptor<TState, TEdge, in TInput> :
    IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>, IAcceptorNavigable<TState, TEdge, TInput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput> { }
```

Here are some sketches of interfaces for transducers.
```cs
public interface ITransducer<in TInput, out TOutput> : IAutomaton<TInput>, ITransducerNavigable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<TState, TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITransducerNavigable<TState, TEdge, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput> { }
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Navigating Automata, Cursors, and Reactive Programming

Interfaces for automata could provide a method, `GetCursor()`, which returns objects for navigating them, objects implementing interfaces including for interoperability with the `System.Reactive` library.

Here are some sketches of a set of `INavigable`-related and `ICursor`-related interfaces.

```cs
public interface ICursor<in TInput> : IObserver<TInput>
{
    public void OnNext(TInput value, out IEnumerable edges);
    public IEnumerable Current { get; }
}
public interface ICursor<TState, TEdge, in TInput> : ICursor<TInput>
{
    public void OnNext(TInput value, out IEnumerable<TEdge> edges);
    public new IEnumerable<TState> Current { get; }
}

public interface INavigable<in TInput>
{
    public ICursor<TInput> GetCursor();
}
public interface INavigable<TState, TEdge, in TInput> : INavigable<TInput>
{
    public new ICursor<TState, TEdge, TInput> GetCursor();
}



public interface IAcceptorCursor<in TInput> : ICursor<TInput>, ISubject<TInput, bool> { }
public interface IAcceptorCursor<TState, TEdge, in TInput> : ICursor<TState, TEdge, TInput>, IAcceptorCursor<TInput> { }

public interface IAcceptorNavigable<in TInput> : INavigable<TInput>
{
    public new IAcceptorCursor<TInput> GetCursor();
}
public interface IAcceptorNavigable<TState, TEdge, in TInput> : INavigable<TState, TEdge, TInput>, IAcceptorNavigable<TInput>
{
    public new IAcceptorCursor<TState, TEdge, TInput> GetCursor();
}



public interface ITransducerCursor<in TInput, out TOutput> : ICursor<TInput>, ISubject<TInput, TOutput> { }
public interface ITransducerCursor<TState, TEdge, in TInput, out TOutput> :
    ICursor<TState, TEdge, TInput>, ITransducerCursor<TInput, TOutput> { }

public interface ITransducerNavigable<in TInput, out TOutput> : INavigable<TInput>
{
    public new ITransducerCursor<TInput, TOutput> GetCursor();
}
public interface ITransducerNavigable<TState, TEdge, in TInput, out TOutput> :
    INavigable<TState, TEdge, TInput>, ITransducerNavigable<TInput, TOutput>
{
    public new ITransducerCursor<TState, TEdge, TInput, TOutput> GetCursor();
}
```

For developer convenience, default implementations of automata traversers can be provided.

## Language Integrated Query (LINQ)

_Coming soon._

## Learning, Building, and Optimizing Automata

_Coming soon._

## Tree Automata

Here are some sketches of interfaces for top-down and bottom-up tree acceptors.

### Top-down

```cs
public interface ITopDownTreeAcceptor<in TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public IEnumerable Start { get; }

    public bool Accepts(TTree tree);
}

public interface ITopDownTreeAcceptor<TRule, TState, in TTree> : ITopDownTreeAcceptor<TTree>
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
public interface IBottomUpTreeAcceptor<in TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public Func<TTree, object> KeySelector { get; }

    public bool Accepts(TTree tree);
}

public interface IBottomUpTreeAcceptor<TRule, TState, in TTree> : IBottomUpTreeAcceptor<TTree>
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
