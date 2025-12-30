# Differencing

An interesting scenario to consider is one where systems' states or their state-related data are [differenceable](https://en.wikipedia.org/wiki/Data_differencing).

```cs
public interface IDifferenceable<in T, out TDifference>
{
    public TDifference DifferenceFrom(T other);
}
```

The above concept is generally useful in combination with reactive programming, e.g., the `System.Reactive` library. One could also use it with enumerable differences, `IDifferenceable<T, IEnumerable<Event>>`.

Developers and end-users could use the above interface to explore differences or deltas between states or their state-related data, e.g., for consecutive states, as input sequences were incrementally presented to systems.

A variety of [automaton navigator](/Content/Automata.md#navigating-automata), [outputting data navigators](/Content/Automata.md#navigating-automata-and-data), could be utilized for these purposes.

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
