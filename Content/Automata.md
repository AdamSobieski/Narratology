## Introduction

### Automata

Here are some sketches of interfaces for automata.

```cs
public interface IAutomaton<out TState, out TEdge, in TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>
{
    IEnumerable<TState> Start { get; }
}

public interface IAutomaton<out TState, out TEdge, in TInput, out TOutput> : IAutomaton<TState, TEdge, TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>
{

}
```

As considered, extension methods would provide functionalities for determining whether an automaton accepts an enumerable of type `TInput`, transducing from such an input sequence to an output sequence of type `TOutput`, and so forth.

Many automata would additionally implement `IMatcher<TInput>` for their `TEdge` type; this type constraint is checked for and utilized by many of the provided extension methods. Transducers would, similarly, additionally implement `IProducer<TInput, TOutput>` on their `TEdge` type.

Automata instances can also implement interfaces to provide their own customized implementations for those functionalities otherwise provided by extension methods, e.g., `ICustomAccepts` and `ICustomTransduce`. Such interfaces can be checked for upon instances in provided extension methods.

A vision is that `IAutomatonBuilder` interfaces or `AutomatonBuilder` (static) classes would enable developers to simply and programmatically build various kinds of automata. Automata builders might configurably optimize developers' described automata and/or utilize runtime code-generation and compiling-related features to maximize performance.

### Tree Automata

Here are some sketches of interfaces for top-down and bottom-up tree automata.

#### Top-down

```cs
public interface ITopDownTreeAutomaton<TRule, TState, TTree>
    where TRule : ITopDownTreeAutomatonRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<TState, IEnumerable<TRule>> Rules { get; }
    public IEnumerable<TState> Start { get; }
}

public interface ITopDownTreeAutomatonRule<out TState, in TTree> : IMatcher<TTree>
{
    public TState Input { get; }
    public IReadOnlyList<TState> Output { get; }
}
```

#### Bottom-up

```cs
public interface IBottomUpTreeAutomaton<TRule, TState, TTree>
    where TRule : IBottomUpTreeAutomatonRule<TState, TTree>
    where TTree : IHasChildren<TTree>
{
    public IReadOnlyDictionary<object, IEnumerable<TRule>> Rules { get; }
    public Func<TTree, object> KeySelector { get; }
}

public interface IBottomUpTreeAutomatonRule<out TState, in TTree> : IMatcher<TTree>
{
    public IReadOnlyList<TState> Input { get; }
    public TState Output { get; }
}
```
