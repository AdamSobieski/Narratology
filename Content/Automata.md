# Automata

## Automata, Acceptors, and Transducers

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton<in TInput> : INavigable<TInput>
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>, INavigable<TState, TEdge, TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>, IMatcher<TInput>
{
    public new IEnumerable<TState> Start { get; }
}
```

Here are some sketches of interfaces for acceptors.

```cs
public interface IAcceptor<in TInput> :
    IAutomaton<TInput>, IAcceptorNavigable<TInput>
{
    public bool Accepts(IEnumerable<TInput> sequence);
}

public interface IAcceptor<out TState, out TEdge, in TInput> :
    IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>, IAcceptorNavigable<TState, TEdge, TInput>
        where TState : IHasOutgoingEdges<TEdge>, IHasIsValid
        where TEdge : IHasTarget<TState>, IMatcher<TInput> { }
```

Here are some sketches of interfaces for transducers.
```cs
public interface ITransducer<in TInput, out TOutput> :
    IAutomaton<TInput>, ITransducerNavigable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<out TState, out TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITransducerNavigable<TState, TEdge, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput> { }
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Navigating Automata

Automaton interfaces extend `INavigable`-related interfaces to provide a method, `GetNavigator()`. This method return an object for navigating the automaton. Automaton navigators, extending `INavigator`-related interfaces, are interoperable with the `System.Reactive` library and reactive programming.

Here are some sketches of a set of `INavigable`-related and `INavigator`-related interfaces.

```cs
public interface INavigator : IDisposable
{
    public IEnumerable Current { get; }
    public IEnumerable Edges { get; }
}
public interface INavigator<in TInput> : INavigator, IObserver<TInput> { }
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
public interface IAcceptorNavigator<in TInput> : INavigator<TInput>, ISubject<TInput, bool> { }
public interface IAcceptorNavigator<out TState, out TEdge, in TInput> :
    INavigator<TState, TEdge, TInput>, IAcceptorNavigator<TInput> { }

public interface IAcceptorNavigable<in TInput> : INavigable<TInput>
{
    public new IAcceptorNavigator<TInput> GetNavigator();
}
public interface IAcceptorNavigable<out TState, out TEdge, in TInput> :
    INavigable<TState, TEdge, TInput>, IAcceptorNavigable<TInput>
{
    public new IAcceptorNavigator<TState, TEdge, TInput> GetNavigator();
}
```
```cs
public interface ITransducerNavigator<in TInput, out TOutput> : INavigator<TInput>, ISubject<TInput, TOutput> { }
public interface ITransducerNavigator<out TState, out TEdge, in TInput, out TOutput> :
    INavigator<TState, TEdge, TInput>, ITransducerNavigator<TInput, TOutput> { }

public interface ITransducerNavigable<in TInput, out TOutput> : INavigable<TInput>
{
    public new ITransducerNavigator<TInput, TOutput> GetNavigator();
}
public interface ITransducerNavigable<out TState, out TEdge, in TInput, out TOutput> :
    INavigable<TState, TEdge, TInput>, ITransducerNavigable<TInput, TOutput>
{
    public new ITransducerNavigator<TState, TEdge, TInput, TOutput> GetNavigator();
}
```

For developer convenience, default implementations of automaton navigators can be provided.

## Navigating Automata and Data

Automaton navigators can carry data. This data could be cloned to be processed by multiple edges. This processed data could be consumed by subsequent states to produce other data. For non-deterministic cases, implementations would need merging or aggregation algorithms for when multiple edges converge onto states.

```cs
public interface IDataNavigator<in TInput, TValue> : INavigator<TInput>
{
    public IReadOnlyDictionary<object, TValue> Data { get; }
}
public interface IDataNavigator<TState, out TEdge, in TInput, TValue> :
    INavigator<TState, TEdge, TInput>, IDataNavigator<TInput, TValue>
{
    public new IReadOnlyDictionary<TState, TValue> Data { get; }
}

public interface IDataNavigable<in TInput, TValue> : INavigable<TInput>
{
    public new IDataNavigator<TInput, TValue> GetNavigator();
}
public interface IDataNavigable<TState, out TEdge, in TInput, TValue> :
    INavigable<TState, TEdge, TInput>, IDataNavigable<TInput, TValue>
{
    public new IDataNavigator<TState, TEdge, TInput, TValue> GetNavigator();
}
```

Possibilities for data to be carried by automaton navigators (i.e., `TValue`) include: `ExpandoObject`, `IReadOnlyDictionary<string, object?>`, and knowledge graphs.

Automaton navigators carrying data could also stream outputs of a specified type, `TOutput`.

```cs
public interface IOutputtingDataNavigator<in TInput, TValue, out TOutput> :
    IDataNavigator<TInput, TValue>, ISubject<TInput, TOutput> { } 
public interface IOutputtingDataNavigator<TState, out TEdge, in TInput, TValue, out TOutput> :
    IDataNavigator<TState, TEdge, TInput, TValue>, IOutputtingDataNavigator<TInput, TValue, TOutput> { }

public interface IOutputtingDataNavigable<in TInput, TValue, out TOutput> : IDataNavigable<TInput, TValue>
{
    public new IOutputtingDataNavigator<TInput, TValue, TOutput> GetNavigator();
}
public interface IOutputtingDataNavigable<TState, out TEdge, in TInput, TValue, out TOutput> :
    IDataNavigable<TState, TEdge, TInput, TValue>, IOutputtingDataNavigable<TInput, TValue, TOutput>
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

# Tree Automata

Here are some sketches of interfaces for top-down and bottom-up tree acceptors.

## Top-down

```cs
public interface ITopDownTreeAcceptor<in TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public IEnumerable Start { get; }

    public bool Accepts(TTree tree);
}

public interface ITopDownTreeAcceptor<TState, TRule, in TTree> : ITopDownTreeAcceptor<TTree>
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

## Bottom-up

```cs
public interface IBottomUpTreeAcceptor<in TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public Func<TTree, object> KeySelector { get; }

    public bool Accepts(TTree tree);
}

public interface IBottomUpTreeAcceptor<out TState, TRule, in TTree> : IBottomUpTreeAcceptor<TTree>
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
