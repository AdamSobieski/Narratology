## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton<in TInput> : INavigable<TInput>
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>, INavigable<TState, TInput>
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
    IAutomaton<TState, TEdge, TInput>, IAcceptor<TInput>, IAcceptorNavigable<TState, TInput>
        where TState : IHasOutgoingEdges<TEdge>, IHasIsValid
        where TEdge : IHasTarget<TState>, IMatcher<TInput> { }
```

Here are some sketches of interfaces for transducers.
```cs
public interface ITransducer<in TInput, out TOutput> : IAutomaton<TInput>, ITransducerNavigable<TInput, TOutput>
{
    public IEnumerable<TOutput> Transduce(IEnumerable<TInput> sequence);
}

public interface ITransducer<TState, TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITransducerNavigable<TState, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput> { }
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Navigating Automata and Reactive Programming

Interfaces for automata could provide a method, `GetNavigator()`, which returns objects for navigating them, objects implementing interfaces including for interoperability with the `System.Reactive` library.

Here are some sketches of a set of `INavigable`-related and `INavigator`-related interfaces.

```cs
public interface INavigator<in TInput> : IObserver<TInput>, IDisposable
{
    public IEnumerable Current { get; }
}
public interface INavigator<out TState, in TInput> : INavigator<TInput>
{
    public new IEnumerable<TState> Current { get; }
}

public interface INavigable<in TInput>
{
    public INavigator<TInput> GetNavigator();
}
public interface INavigable<out TState, in TInput> : INavigable<TInput>
{
    public new INavigator<TState, TInput> GetNavigator();
}
```
```cs
public interface IAcceptorNavigator<in TInput> : INavigator<TInput>, ISubject<TInput, bool> { }
public interface IAcceptorNavigator<out TState, in TInput> : INavigator<TState, TInput>, IAcceptorNavigator<TInput> { }

public interface IAcceptorNavigable<in TInput> : INavigable<TInput>
{
    public new IAcceptorNavigator<TInput> GetNavigator();
}
public interface IAcceptorNavigable<out TState, in TInput> : INavigable<TState, TInput>, IAcceptorNavigable<TInput>
{
    public new IAcceptorNavigator<TState, TInput> GetNavigator();
}
```
```cs
public interface ITransducerNavigator<in TInput, out TOutput> : INavigator<TInput>, ISubject<TInput, TOutput> { }
public interface ITransducerNavigator<out TState, in TInput, out TOutput> :
    INavigator<TState, TInput>, ITransducerNavigator<TInput, TOutput> { }

public interface ITransducerNavigable<in TInput, out TOutput> : INavigable<TInput>
{
    public new ITransducerNavigator<TInput, TOutput> GetNavigator();
}
public interface ITransducerNavigable<out TState, in TInput, out TOutput> :
    INavigable<TState, TInput>, ITransducerNavigable<TInput, TOutput>
{
    public new ITransducerNavigator<TState, TInput, TOutput> GetNavigator();
}
```

For developer convenience, default implementations of automaton navigators can be provided.

## Navigating Automata and Data

Automaton navigators can carry data. This data could be cloned and processed by edges. This processed data could be consumed by states to produce other data. For non-deterministic cases, implementations would need merging or aggregation algorithms for when multiple edges converge onto states.

```cs
public interface IDataNavigator<in TInput, TValue> : INavigator<TInput>, ISubject<TInput, IEnumerable<TValue>>
{
    public IReadOnlyDictionary<object, TValue> Data { get; }
}
public interface IDataNavigator<TState, in TInput, TValue> : INavigator<TState, TInput>, IDataNavigator<TInput, TValue>
{
    public new IReadOnlyDictionary<TState, TValue> Data { get; }
}

public interface IDataNavigable<in TInput, TValue> : INavigable<TInput>
{
    public new IDataNavigator<TInput, TValue> GetNavigator();
}
public interface IDataNavigable<TState, in TInput, TValue> : INavigable<TState, TInput>, IDataNavigable<TInput, TValue>
{
    public new IDataNavigator<TState, TInput, TValue> GetNavigator();
}
```

Possibilities for data to be carried by automaton navigators (i.e., `TValue`) include: `ExpandoObject`, `IReadOnlyDictionary<string, object?>`, and knowledge graphs.

## Language Integrated Query (LINQ)

Method chaining via extension methods could utilize automata and their navigators.

```cs
public static IEnumerable<TInput> Where<TState, TInput>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Func<INavigator<TState, TInput>, TInput, bool> functor
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
public static IEnumerable<TResult> Select<TState, TInput, TResult>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Func<INavigator<TState, TInput>, TInput, TResult> selector
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
public static void Do<TState, TInput>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Action<INavigator<TState, TInput>, TInput> action
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
