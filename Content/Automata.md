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

For developer convenience, default implementations of `Accepts()` and `Transduce()` could be provided as static methods.

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

## Navigating Automata and Data

Automaton navigators can carry data. This data could be cloned to be processed by multiple edges. This processed data could be consumed by subsequent states to produce other data. For non-deterministic cases, implementations would need merging or aggregation algorithms for when multiple edges converge onto states.

```cs
public interface IDataNavigator : INavigator
{
    public object GetData(object state);
}

public interface IDataNavigator<in TInput, out TValue> : IDataNavigator, INavigator<TInput>
{
    public new TValue GetData(object state);
}

public interface IDataNavigator<TState, out TEdge, in TInput, out TValue> :
    IDataNavigator<TInput, TValue>, INavigator<TState, TEdge, TInput>
{
    public TValue GetData(TState state);
}

public interface IDataNavigable : INavigable
{
    public new IDataNavigator GetNavigator();
}

public interface IDataNavigable<in TInput, out TValue> : IDataNavigable, INavigable<TInput>
{
    public new IDataNavigator<TInput, TValue> GetNavigator();
}

public interface IDataNavigable<TState, out TEdge, in TInput, out TValue> :
    IDataNavigable<TInput, TValue>, INavigable<TState, TEdge, TInput>
{
    public new IDataNavigator<TState, TEdge, TInput, TValue> GetNavigator();
}
```

Possibilities for data to be carried by automaton navigators (i.e., `TValue`) include: `ExpandoObject`, `IReadOnlyDictionary<string, object?>`, and knowledge graphs.

Automaton navigators carrying data could also stream outputs of a specified type, `TOutput`.

```cs
public interface IOutputtingDataNavigator : IDataNavigator { }

public interface IOutputtingDataNavigator<in TInput, out TValue, out TOutput> :
    IOutputtingDataNavigator, IDataNavigator<TInput, TValue>, ISubject<TInput, TOutput> { } 

public interface IOutputtingDataNavigator<TState, out TEdge, in TInput, out TValue, out TOutput> :
    IOutputtingDataNavigator<TInput, TValue, TOutput>, IDataNavigator<TState, TEdge, TInput, TValue> { }

public interface IOutputtingDataNavigable : IDataNavigable
{
    public new IOutputtingDataNavigator GetNavigator();
}

public interface IOutputtingDataNavigable<in TInput, out TValue, out TOutput> :
    IOutputtingDataNavigable, IDataNavigable<TInput, TValue>
{
    public new IOutputtingDataNavigator<TInput, TValue, TOutput> GetNavigator();
}

public interface IOutputtingDataNavigable<TState, out TEdge, in TInput, out TValue, out TOutput> :
    IOutputtingDataNavigable<TInput, TValue, TOutput>, IDataNavigable<TState, TEdge, TInput, TValue>
{
    public new IOutputtingDataNavigator<TState, TEdge, TInput, TValue, TOutput> GetNavigator();
}
```

## Language Integrated Query (LINQ)

Method chaining via extension methods could utilize automata and their navigators.

```cs
public static IEnumerable<TInput> Where<TState, TEdge, TInput>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TEdge, TInput> navigable,
    Func<INavigator<TState, TEdge, TInput>, TInput, bool> functor
)
{
    using (var navigator = navigable.GetNavigator())
    {
        foreach (var element in source)
        {
            bool b = false;

            try
            {
                navigator.OnNext(element);
                b = functor(navigator, element);
            }
            catch (Exception ex)
            {
                navigator.OnError(ex);
                break;
            }
            if (b) yield return element;
        }
        navigator.OnCompleted();
    }
}
```
```cs
public static IEnumerable<TResult> Select<TState, TEdge, TInput, TResult>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TEdge, TInput> navigable,
    Func<INavigator<TState, TEdge, TInput>, TInput, TResult> selector
)
{
    using (var navigator = navigable.GetNavigator())
    {
        foreach (var element in source)
        {
            TResult result;
            try
            {
                navigator.OnNext(element);
                result = selector(navigator, element);
            }
            catch (Exception ex)
            {
                navigator.OnError(ex);
                break;
            }
            yield return result;
        }
        navigator.OnCompleted();
    }
}
```
```cs
public static void Do<TState, TEdge, TInput>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TEdge, TInput> navigable,
    Action<INavigator<TState, TEdge, TInput>, TInput> action
)
{
    using (var navigator = navigable.GetNavigator())
    {
        foreach (var element in source)
        {
            try
            {
                navigator.OnNext(element);
                action(navigator, element);
            }
            catch (Exception ex)
            {
                navigator.OnError(ex);
                break;
            }
        }
        navigator.OnCompleted();
    }
}
```

## Casting Automata and Other Navigables to Different Types

A variety of extension methods for casting navigables to different types, i.e., `Cast<...>()`, can be developed, some involving wrappers.
