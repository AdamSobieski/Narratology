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

## Rule-building Interfaces

```cs
public interface IRuleBuilder
{
    public IRuleBuilder ForAll<X>(Expression<Action<IRuleBuilder<X>>> action);
    
    LambdaExpression Build();
}

public interface IRuleBuilder<X>
{
    public void Rule(Expression<Func<IKnowledge, X, bool>> antecedent, Expression<Func<IKnowledge, X, bool>> consequent);

    Expression<Action<X>> GetLambdaExpression();
}
```

## Rule-building Syntax Example

```cs
var rule = rb.ForAll<(Person a, Person b, Person c)>(b => b.Rule((kb, v) => kb.FatherOf(v.a, v.c) & kb.BrotherOf(v.a, v.b), (kb, v) => kb.UncleOf(v.b, v.c))).Build();
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

## Knowledgebase API

Here is a knowledgebase interface. Note that `ParameterExpression` instances could be used as variables with some methods, e.g., `Search()`.

```cs
public interface IKnowledge
{
    public void Assert(MethodBase predicate, object?[] arguments);

    public bool Contains(MethodBase predicate, object?[] arguments);

    public bool Entails(MethodBase predicate, object?[] arguments);

    public void Retract(MethodBase predicate, object?[] arguments);

    public void Assert(LambdaExpression rule);

    public bool Contains(LambdaExpression rule);

    public void Retract(LambdaExpression rule);

    public IQueryable<(MethodBase Predicate, object?[] Arguments)> Search(object predicate, object?[] arguments);

    public IQueryable<X> Query<X>(params Expression<Func<IKnowledge, X, bool>>[] query);
}
```

### Query Example

```cs
kb.Query<(Person x, Person y)>((kb, v) => kb.BrotherOf(Alex, v.x), (kb, v) => kb.FatherOf(v.x, v.y)).Select(v => v.y);
```
