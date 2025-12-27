## Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton<in TInput> : INavigable<TInput>
{
    public IEnumerable Start { get; }
}

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>, INavigable<TState, TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>
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
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>, ITransducerNavigable<TState, TInput, TOutput>
        where TState : IHasOutgoingEdges<TEdge>
        where TEdge : IHasTarget<TState>, IMatcher<TInput>, IProducer<TInput, TOutput> { }
```

For developer convenience, default implementations of `Accepts()` and `Transduce()` can be provided as static methods.

## Navigating Automata and Reactive Programming

Interfaces for automata could provide a method, `GetNavigator()`, which returns objects for navigating them, objects implementing interfaces including for interoperability with the `System.Reactive` library.

Here are some sketches of a set of `INavigable`-related and `INavigator`-related interfaces.

```cs
public interface INavigator<in TInput> : IObserver<TInput>
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

## Language Integrated Query (LINQ)

Method chaining can utilize the contextual states of automata navigators. As `TState` would be defined by developers, navigators' states could provide either acontextual or contextual methods for filtering or transforming elements in enumerables or streams.

```cs
public static IEnumerable<TInput> Where<TState, TInput>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Func<IEnumerable<TState>, TInput, bool> functor
)
{
    var traverser = navigable.GetNavigator();

    foreach (var element in source)
    {
        traverser.OnNext(element);

        if (functor(traverser.Current, element))
        {
            yield return element;
        }
    }

    traverser.OnCompleted();
}
```

```cs
public static IEnumerable<TResult> Select<TState, TInput, TResult>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Func<IEnumerable<TState>, TInput, TResult> selector
)
{
    var traverser = navigable.GetNavigator();

    foreach (var element in source)
    {
        traverser.OnNext(element);
        yield return (selector(traverser.Current, element));
    }

    traverser.OnCompleted();
}
```

```cs
public static void Do<TState, TInput, TResult>
(
    this IEnumerable<TInput> source,
    INavigable<TState, TInput> navigable,
    Action<IEnumerable<TState>, TInput> action
)
{
    var traverser = navigable.GetNavigator();

    foreach (var element in source)
    {
        traverser.OnNext(element);
        action(traverser.Current, element);
    }

    traverser.OnCompleted();
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

## Learning, Building, and Optimizing Automata

_Coming soon._
