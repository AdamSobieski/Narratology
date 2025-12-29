# Differencing

An interesting scenario to consider is one where systems' states or their state-related data are [differenceable](https://en.wikipedia.org/wiki/Data_differencing).

```cs
public interface IDifferenceable<in T, out TDifference>
{
    public TDifference DifferenceFrom(T other);
}
```

While generally useful in combination with reactive programming, e.g., `System.Reactive`, developers and end-users could use the above interface to explore differences or deltas between states or their state-related data, e.g., for consecutive states, as input sequences were incrementally presented to systems.

A new variety of [automaton navigator](/Content/Automata.md#navigating-automata), perhaps one extending [data navigators](/Content/Automata.md#navigating-automata-and-data), could be developed for these scenarios.

Related interfaces include those with which to enable objects to process such differences, either to be modified in-place or into cloned copies.

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
