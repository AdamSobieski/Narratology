# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, predicate-calculus expressions, and logical rules.

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

## Rule Building Interfaces

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

## Rule Building Syntax Example

```cs
var rule = rb.ForAll<(Person a, Person b, Person c)>(b => b.Rule((kb, v) => kb.FatherOf(v.a, v.c) && kb.BrotherOf(v.a, v.b), (kb, v) => kb.UncleOf(v.b, v.c))).Build();
```
