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
{

}
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
{

}
```

## Navigating Automata

Automaton interfaces extend a set of `INavigable`-related interfaces to provide a method, `GetNavigator()`. This method returns objects for navigating automata. Automaton navigators are interoperable with the `System.Reactive` library for reactive programming.

```cs
public interface INavigator : IHasConstraints, IDisposable
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

public interface INavigable : IHasConstraints
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

## Extension Methods

Some extension methods for automata will use type constraints like:

```cs
where TState : IHasOutgoingEdges<TEdge>
where TEdge : IHasTarget<TState>, IMatcher<TInput>
```

Some for acceptors:
```cs
where TState : IHasOutgoingEdges<TEdge>, IAccepts
where TEdge : IHasTarget<TState>, IMatcher<TInput>
```

Some for transducers:
```cs
where TState : IHasOutgoingEdges<TEdge>
where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput>
```

## Casting

A variety of extension methods for casting navigables to different types, i.e., `Cast<...>()`, can be provided.

## Constraints
See Also: _[Constraints](Constraints.md)_

An inspectable constraints system, utilizing the abstract syntax tree model from the `System.Linq.Expressions` namespace, will allow developers to easily express logical constraints, or invariants, which are to hold upon objects, e.g., navigables and their navigators, and to easily express logical constraints involving progressions through states and edges during navigations.

An inspectable constraints system will enable many useful features, simplify debugging scenarios, and enable descriptive extension members like `bool IsDeterministic { get; }`.
