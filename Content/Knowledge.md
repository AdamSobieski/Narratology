# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, propositional-logical expressions, and logical rules.

## Predicates as Extension Methods of Knowledgebases

The following example shows how predicates can be represented as simple extension methods which extend a knowledgebase type.

```cs
public static partial class ExampleModule
{
    [Predicate]
    public static bool FatherOf(this IKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool BrotherOf(this IKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool UncleOf(this IKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }
}
```

## Knowledgebases

Here is a knowledgebase interface. `ParameterExpression` instances could be used as variables with some methods, e.g., `Match()`. This interface is a work in progress; it will, eventually, include a means of loading data and rules from stored resources. Some of the features indicated, below, may, instead, be made available as extension methods on the `IKnowledge` interface.

```cs
public interface IKnowledge
{
    public void Assert(MethodBase predicate, object?[] arguments);

    public bool Contains(MethodBase predicate, object?[] arguments);

    public bool Entails(MethodBase predicate, object?[] arguments);

    public void Retract(MethodBase predicate, object?[] arguments);

    public IQueryable<(MethodBase Predicate, object?[] Arguments)> Search(object predicate, object?[] arguments);

    public void Assert<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent);

    public bool Contains<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent);

    public void Retract<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent);

    public IQueryable<X> Query<X>(params Expression<Func<IKnowledge, X, bool>>[] query);
}
```

### Rule-assertion Example

```cs
kb.Assert<(Person a, Person b, Person c)>((kb, v) => kb.UncleOf(v.b, v.c), (kb, v) => kb.FatherOf(v.a, v.c), (kb, v) => kb.BrotherOf(v.a, v.b));
```

### Query Example

```cs
Person alex = new Person("Alex Smith");

kb.Query<(Person x, Person y)>((kb, v) => kb.BrotherOf(alex, v.x), (kb, v) => kb.FatherOf(v.x, v.y)).Select(v => v.y);
```

## Second-order Logic and Recursive Expressiveness

If the above is analogous to a Datalog level of expressiveness, the following would be steps to enable Prolog expressiveness, and beyond.

In the system considered above, special types could include delegate types, which, as variable types, could enable second-order expressions, and lambda expressions, e.g., `Expression<Func<IKnowledge, bool>>`, which, when used as predicates' parameters, would enable a recursive expressiveness.

Here is an example of a second-order expression, a rule with a quantified variable of a delegate type:
```cs
var rule2 = rb.ForAll<(Func<IKnowledge, Person, Person, bool> P, Person x, Person y)>(...).Build();
```

Here is an example of a predicate which could receive expressions as arguments:
```cs
[Predicate]
public static bool Meta(this IKnowledge kb, Expression<Func<IKnowledge, bool>> x, Expression<Func<IKnowledge, bool>> y)
{
    return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
}
```
