## Computational Narratology and Biography

Considered, here, are some implementational approaches to computational narratology and biography.

### Language Integrated Query and Events

With language integrated query (LINQ), we could consider queryable collections of events, `IQueryable<IEvent>`. The specifics of any `IEvent` interface would prove to be important; let us, initially, consider a simple model where events can have one or more categories.

```cs
public partial interface IEvent
{
    public IEnumerable<ICategory> Categories { get; }

    public DateTime Start { get; }
    public DateTime? End { get; }

    public string? Name { get; }
    public string Description { get; }

    public bool About(IPerson person);
    public bool Mentions(IPerson person);
}
```

Next, we could provide an extension function, `GroupByMany()`:

```cs
public static partial class Extensions
{
    public static IEnumerable<IGrouping<TKey, TElement>> GroupByMany<TKey, TElement>(this IEnumerable<TElement> source, Func<TElement, IEnumerable<TKey>> keysSelector)
    {
        return source.SelectMany(element => keysSelector(element).Select(key => new { Key = key, Element = element })).GroupBy(pair => pair.Key, pair => pair.Element);
    }
}
```

so that we might more readily express:

```cs
IEnumerable<IGrouping<ICategory, IEvent>> data = events.Where(e => e.About(person)).OrderBy(e => e.Start).GroupByMany(e => e.Categories);
```

If events' categories were hierarchical in nature, capable of having super-categories and sub-categories, then one could create hierarchical, chronologically-sorted, multi-track, timeline-based, event-related data structures.
