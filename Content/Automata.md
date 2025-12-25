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

public interface ITransducer : IAutomaton
{
    public Type OutputType { get; }
}

public interface IAutomaton<in TInput> : IAutomaton
{

}

public interface ITransducer<in TInput, out TOutput> : IAutomaton<TInput>, ITransducer
{

}

public interface IAutomaton<out TState, out TEdge, in TInput> : IAutomaton<TInput>
    where TState : IHasOutgoingEdges<TEdge>
    where TEdge : IHasTarget<TState>
{
    public new IEnumerable<TState> Start { get; }
}

public interface ITransducer<out TState, out TEdge, in TInput, out TOutput> :
    IAutomaton<TState, TEdge, TInput>, ITransducer<TInput, TOutput>
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
