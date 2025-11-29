## Computational Narratology and Biography

Considered, here, are computational narratological and biographical approaches primarily involving queryable sets of events.

```cs
IQueryable<IEvent> events;
```

The composition of the `IEvent` interface will prove to be important. Let us, initially, consider a simple model where events have categories.

```cs
public partial interface IEvent
{
    public IEnumerable<IEventCategory> Categories { get; }

    public DateTime Start { get; }
    public DateTime? End { get; }

    public string? DisplayName { get; }

    public string Description { get; }

    public bool Involves(IPerson person);
}
```

Next, we can provide an extension function, `GroupByMany()`:

```cs
public static partial class Extensions
{
    public static IEnumerable<IGrouping<TKey, TElement>> GroupByMany<TKey, TElement>(this IEnumerable<TElement> source, Func<TElement, IEnumerable<TKey>> keysSelector)
    {
        return source.SelectMany(element => keysSelector(element).Select(key => new { Key = key, Element = element })).GroupBy(pair => pair.Key, pair => pair.Element);
    }
}
```

so that we can express:

```cs
IEnumerable<IGrouping<IEventCategory, IEvent>> x = events.Where(e => e.Involves(person)).GroupByMany(e => e.Categories);
```

If events' categories were hierarchical in nature, capable of having super-categories and sub-categories, then we could consider tree-hierarchical, chronologically-sorted, multi-track, timeline views of life events.

That is, there would be a relationship between `IEnumerable<IGrouping<IEventCategory, IEvent>>` and the following example diagram:

```
+-------------+----------------+-----------------+------------------------
|             |                | Subcategory #5  | [Event #1][Event #2]
|             | Subcategory #1 |-----------------+------------------------
|             |                | Subcategory #6  | [Event #3]
| Category #1 |----------------+-----------------+------------------------
|             |                | Subcategory #7  |            [Event #4]
|             | Subcategory #2 |-----------------+------------------------
|             |                | Subcategory #8  |      [Event #5     ]
+-------------+----------------+-----------------+------------------------
|             |                | Subcategory #9  |   [Event #6          ]
|             | Subcategory #3 |-----------------+------------------------
|             |                | Subcategory #10 | [Event #3][Event# 7]
| Category #2 |----------------+-----------------+------------------------
|             |                | Subcategory #11 |            [Event #8]
|             | Subcategory #4 |-----------------+------------------------
|             |                | Subcategory #12 |  [Event #9][Event #10]
+-------------+----------------+-----------------+------------------------
                                                 |1|1|1|1|1|1|1|1|1|1|1|1|
                                                 |9|9|9|9|9|9|9|9|9|9|9|9| 
                                                 |5|5|5|5|5|5|5|5|5|5|6|6| ...
                                                 |0|1|2|3|4|5|6|7|8|9|0|1|
```
