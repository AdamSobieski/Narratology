# Differencing

An interesting scenario to consider is one where systems' states or their state-related data are [differenceable](https://en.wikipedia.org/wiki/Data_differencing).

```cs
public interface IDifferenceable<in T, out TDifference>
{
    public TDifference DifferenceFrom(T other);
}
```

Developers and end-users could explore differences between states or state-related data, e.g., for consecutive states, as input sequences were incrementally presented to systems.

Related interfaces include those to enable objects to process such differences, either to be modified in-place or into cloned copies.

```cs
public interface IApplyDifference<in TDifference>
{
    public void Apply(TDifference difference);
}

public interface ICloneable<in TDifference> : ICloneable
{
    public object Clone(TDifference difference);
}
```

A new variety of [automaton navigator](/Content/Automata.md#navigating-automata) could be developed for these scenarios.
