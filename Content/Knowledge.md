# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, propositional-logical expressions, and logical rules.

## Predicates as Extension Methods of Knowledgebases

The following example shows how predicates can be represented as simple extension methods that extend a knowledgebase type.

```cs
public static partial class ExampleModule
{
    public static bool FatherOf(this IKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }
    public static bool BrotherOf(this IKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }
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

In the considered system, special types might include certain delegate types, which, as variable types, could enable second-order expressions, and `Expression<Func<IKnowledge, bool>>` which, when used as predicates' parameters, could enable recursive expressiveness.

Here is an example of a second-order expression, a rule with a quantified variable of a delegate type:
```cs
var rule2 = rb.ForAll<(Func<IKnowledge, Person, Person, bool> P, Person x, Person y)>(...).Build();
```

Here is an example of a predicate which could receive expressions as arguments:
```cs
public static bool Meta(this IKnowledge kb, Expression<Func<IKnowledge, bool>> x1, Expression<Func<IKnowledge, bool>> x2)
{
    return kb.Entails(MethodBase.GetCurrentMethod()!, [x1, x2]);
}
```
