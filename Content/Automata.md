## Introduction

### Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton
{
    public Type InputType { get; }
    public Type StateType { get; }
    public Type EdgeType { get; }

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
    public Type OutputType { get; }

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

For developer convenience, default implementations of `Accept()` and `Transduce()` could be provided as static methods.

### Traversing Automata

Interfaces for acceptors and, in particular, transducers could provide a method, `GetTraverser()`, which returns an object for traversing the automata, one implementing interfaces extending `IObserver<TInput>`, `IObservable<TOutput>`, and `ISubject<TInput, TOutput>` for interoperability with the `System.Reactive` library.

### Tree Automata

Here are some sketches of interfaces for top-down and bottom-up tree automata.

#### Top-down

```cs
public interface ITopDownTreeAutomaton
{
    public Type TreeType { get; }
    public Type StateType { get; }
    public Type RuleType { get; }

    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public IEnumerable Start { get; }
}

public interface ITopDownTreeAutomaton<TTree> : ITopDownTreeAutomaton
    where TTree : IHasChildren<TTree>
{

}

public interface ITopDownTreeAutomaton<TRule, TState, TTree> : ITopDownTreeAutomaton<TTree>
    where TRule : ITopDownTreeAutomatonRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public new IReadOnlyDictionary<TState, IEnumerable<TRule>> Rules { get; }
    public new IEnumerable<TState> Start { get; }
}

public interface ITopDownTreeAutomatonRule<out TState, in TTree> : IMatcher<TTree>
{
    public TState Input { get; }
    public IReadOnlyList<TState> Output { get; }
}
```

#### Bottom-up

```cs
public interface IBottomUpTreeAutomaton
{
    public Type TreeType { get; }
    public Type StateType { get; }
    public Type RuleType { get; }

    public IReadOnlyDictionary<object, IEnumerable> Rules { get; }
    public Func<object, object> KeySelector { get; }
}

public interface IBottomUpTreeAutomaton<TTree> : IBottomUpTreeAutomaton
    where TTree : IHasChildren<TTree>
{
    public new Func<TTree, object> KeySelector { get; }
}

public interface IBottomUpTreeAutomaton<TRule, TState, TTree> : IBottomUpTreeAutomaton<TTree>
    where TRule : IBottomUpTreeAutomatonRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public new IReadOnlyDictionary<object, IEnumerable<TRule>> Rules { get; }
}

public interface IBottomUpTreeAutomatonRule<out TState, in TTree> : IMatcher<TTree>
{
    public IReadOnlyList<TState> Input { get; }
    public TState Output { get; }
}
```
