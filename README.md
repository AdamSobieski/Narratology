## Computational Narratology and Biography

Considered, here, are computational narratological and biographical approaches primarily involving queryable sets of events.

```cs
IQueryable<IEvent> events;
```

The composition of the `IEvent` interface will prove to be important. Let us, initially, consider a simple model where events have categories.

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
    public bool Involves(IPerson person)
    {
      return this.About(person) || this.Mentions(person);
    }
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

so that we can more readily express:

```cs
IEnumerable<IGrouping<ICategory, IEvent>> x = events.Where(e => e.Involves(person)).OrderBy(e => e.Start).GroupByMany(e => e.Categories);
```

There is a relationship between `IEnumerable<IGrouping<ICategory, IEvent>>` and the following example diagram:

```
+-------------+------------------------
| Category #1 | [Event #1][Event #2]
+-------------+------------------------
| Category #2 | [Event #3]
+-------------+------------------------
| Category #3 |            [Event #4]
+-------------+------------------------
| Category #4 |      [Event #5     ]
+-------------+------------------------
| Category #5 |   [Event #6          ]
+-------------+------------------------
| Category #6 | [Event #3][Event# 7]
+-------------+------------------------
| Category #7 |            [Event #8]
+-------------+------------------------
| Category #8 |  [Event #9][Event #10]
+-------------+------------------------
              |1|1|1|1|1|1|1|1|1|1|1|1|
              |9|9|9|9|9|9|9|9|9|9|9|9| 
              |5|5|5|5|5|5|5|5|5|5|6|6| ...
              |0|1|2|3|4|5|6|7|8|9|0|1|
```

If events' categories were hierarchical in nature, capable of having super-categories and sub-categories, then we could consider hierarchical, chronologically-sorted, multi-track, timeline views of life events:

```
+--------------+--------------+-------------+------------------------
|              |              | Category #1 | [Event #1][Event #2]
|              | Category #9  +-------------+------------------------
|              |              | Category #2 | [Event #3]
| Category #13 +--------------+-------------+------------------------
|              |              | Category #3 |            [Event #4]
|              | Category #10 +-------------+------------------------
|              |              | Category #4 |      [Event #5     ]
+--------------+--------------+-------------+------------------------
|              |              | Category #5 |   [Event #6          ]
|              | Category #11 +-------------+------------------------
|              |              | Category #6 | [Event #3][Event# 7]
| Category #14 +--------------+-------------+------------------------
|              |              | Category #7 |            [Event #8]
|              | Category #12 +-------------+------------------------
|              |              | Category #8 |  [Event #9][Event #10]
+--------------+--------------+-------------+------------------------
                                            |1|1|1|1|1|1|1|1|1|1|1|1|
                                            |9|9|9|9|9|9|9|9|9|9|9|9| 
                                            |5|5|5|5|5|5|5|5|5|5|6|6| ...
                                            |0|1|2|3|4|5|6|7|8|9|0|1|
```
