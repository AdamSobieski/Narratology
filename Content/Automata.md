# Automata

## Automata, Acceptors, and Transducers

Here are some interfaces for automata.

```cs
public interface IAutomaton : INavigable
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<in TInput> : IAutomaton, INavigable<TInput> { }

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>, INavigable<TState, TEdge, TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>, IMatcher<TInput>
{
    public new IEnumerable<TState> Start { get; }
}
```

Here are some interfaces for acceptors.

```cs
public interface IAcceptor : IAutomaton
{
    public bool Accepts(IEnumerable sequence);
}

public interface IAcceptor<in TInput> : 
    IAcceptor, IAutomaton<TInput>, IAcceptorNavigable<TInput>
{
    public bool Accepts(IEnumerable<TInput> sequence);
}

public interface IAcceptor<out TState, out TEdge, in TInput> :
    IAcceptor<TInput>, IAutomaton<TState, TEdge, TInput>, IAcceptorNavigable<TState, TEdge, TInput>
        where TState : IHasOutgoingEdges<TEdge>, IHasIsValid
        where TEdge : IHasTarget<TState>, IMatcher<TInput> { }
```

Here are some interfaces for transducers.
```cs
public interface ITransducer : IAutomaton
{
    public IEnumerable Transduce(IEnumerable sequence);
}

public interface ITransducer<in TInput, out TOutput> :
    ITransducer, IAutomaton<TInput>, ITransducerNavigable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<out TState, out TEdge, in TInput, out TOutput> :
    ITransducer<TInput, TOutput>, IAutomaton<TState, TEdge, TInput>, ITransducerNavigable<TState, TEdge, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput> { }
```

## Navigating Automata

Automaton interfaces extend a set of `INavigable`-related interfaces to provide a method, `GetNavigator()`. This method return an object for navigating the automaton. Automaton navigators, extending a set of `INavigator`-related interfaces, are interoperable with the `System.Reactive` library for reactive programming.

```cs
public interface INavigator : IDisposable
{
    public void OnNext(object value);
    public void OnError(Exception error);
    public void OnCompleted();

    public IEnumerable Current { get; }
    public IEnumerable Edges { get; }
}

public interface INavigator<in TInput> : INavigator, IObserver<TInput>
{
    public new void OnNext(TInput value);
    public new void OnError(Exception error);
    public new void OnCompleted();
}

public interface INavigator<out TState, out TEdge, in TInput> : INavigator<TInput>
{
    public new IEnumerable<TState> Current { get; }
    public new IEnumerable<TEdge> Edges { get; }
}

public interface INavigable
{
    public INavigator GetNavigator();
}

public interface INavigable<in TInput> : INavigable
{
    public new INavigator<TInput> GetNavigator();
}

public interface INavigable<out TState, out TEdge, in TInput> : INavigable<TInput>
{
    public new INavigator<TState, TEdge, TInput> GetNavigator();
}
```
```cs
public interface IAcceptorNavigator : INavigator { }

public interface IAcceptorNavigator<in TInput> :
    IAcceptorNavigator, INavigator<TInput>, ISubject<TInput, bool> { }

public interface IAcceptorNavigator<out TState, out TEdge, in TInput> :
    IAcceptorNavigator<TInput>, INavigator<TState, TEdge, TInput> { }

public interface IAcceptorNavigable : INavigable
{
    public new IAcceptorNavigator GetNavigator();
}

public interface IAcceptorNavigable<in TInput> : IAcceptorNavigable, INavigable<TInput>
{
    public new IAcceptorNavigator<TInput> GetNavigator();
}

public interface IAcceptorNavigable<out TState, out TEdge, in TInput> :
    IAcceptorNavigable<TInput>, INavigable<TState, TEdge, TInput>
{
    public new IAcceptorNavigator<TState, TEdge, TInput> GetNavigator();
}
```
```cs
public interface ITransducerNavigator : INavigator { }

public interface ITransducerNavigator<in TInput, out TOutput> :
    ITransducerNavigator, INavigator<TInput>, ISubject<TInput, TOutput> { }

public interface ITransducerNavigator<out TState, out TEdge, in TInput, out TOutput> :
    ITransducerNavigator<TInput, TOutput>, INavigator<TState, TEdge, TInput> { }

public interface ITransducerNavigable : INavigable
{
    public new ITransducerNavigator GetNavigator();
}

public interface ITransducerNavigable<in TInput, out TOutput> : ITransducerNavigable, INavigable<TInput>
{
    public new ITransducerNavigator<TInput, TOutput> GetNavigator();
}

public interface ITransducerNavigable<out TState, out TEdge, in TInput, out TOutput> :
    ITransducerNavigable<TInput, TOutput>, INavigable<TState, TEdge, TInput>
{
    public new ITransducerNavigator<TState, TEdge, TInput, TOutput> GetNavigator();
}
```

For developer convenience, default implementations of automaton navigators can be provided.

## Navigators Carrying Data

Automaton navigators could carry data or objects. For example, they could provide readonly interfaces for stacks, queues, expando objects, or knowledge graphs.

Towards delivering these capabilities, firstly, `INavigator` could provide a dictionary to return an object for each current state.

```cs
public interface INavigator : IDisposable
{
    public void OnNext(object value);
    public void OnError(Exception error);
    public void OnCompleted();

    public IEnumerable Current { get; }
    public IEnumerable Edges { get; }

    public IReadOnlyDictionary<object, object?> Data { get; }
}
```

A second possibility is that `IHasContextualData` interfaces could be utilized by those implementing state objects and available as a type constraint on `TState` for extension methods.

```cs
public interface IHasContextualData
{
    public object? GetData(INavigator context);
}
public interface IHasContextualData<out TData> : IHasContextualData
{
    public new TData GetData(INavigator context);
}
```

However, with wrappers utilized by type casting (see below), it may be the case that original, navigable-provided navigator objects would be inaccessible to state objects. So, as needed, one could provide an `UnderlyingNavigator` property on `INavigator` to return an underlying, or wrapped, object.

## Casting Automata and Other Navigables to Different Types

A variety of extension methods for casting navigables to different types, i.e., `Cast<...>()`, can be developed, many involving wrappers.
