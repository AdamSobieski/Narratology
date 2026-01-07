# Knowledge

Here are some considered approaches for representing strongly-typed structured knowledge, propositional-logical expressions, rules, and queries.

## Predicates as Extension Methods of Knowledgebases

The following example shows how predicates can be represented as simple extension methods which extend a knowledgebase interface. This technique provides numerous benefits including simplifying organizing collections of predicates, using namespaces, in .NET assemblies. Developers could access their desired predicates, including with IntelliSense features, by using namespaces in source-code files.

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

## Knowledgebase API

This knowledgebase interface, a work in progress, is designed to be general-purpose, enabling developers to work with rules and queries programmatically, using a convenient and approachable C# syntax, while being simultaneously scalable for developers to load and parse collections of rules from resources.

```cs
public interface IKnowledge
{
    public void Assert(MethodBase predicate, object?[] arguments);

    public bool Contains(MethodBase predicate, object?[] arguments);

    public bool Entails(MethodBase predicate, object?[] arguments);

    public void Retract(MethodBase predicate, object?[] arguments);

    public void Assert(Expression rule);

    public bool Contains(Expression rule);

    public void Retract(Expression rule);

    public IQueryable Query(Expression query);
}
```

Other functionalities to be considered include: (1) providing developers with the means to provide `IEqualityComparer` instances for types, and (2) formatting expressions to strings.

### Builtins and Extension Methods for Working with Rules and Queries

The following builtins and extension methods intend to provide developers with a convenient and approachable syntax for working with rules and queries.

```cs
public static partial class Builtin
{
    static MethodInfo _Rule = typeof(Builtin).GetMethod(nameof(Builtin.Rule), BindingFlags.Public | BindingFlags.Static)!;
    static MethodInfo _Query = typeof(Builtin).GetMethod(nameof(Builtin.Query), BindingFlags.Public | BindingFlags.Static)!;

    public static void Rule<X>(Func<IKnowledge, X, bool> consequent, params Func<IKnowledge, X, bool>[] antecedent)
    {

    }
    public static void Query<X>(params Func<IKnowledge, X, bool>[] antecedent)
    {

    }

    extension(IKnowledge kb)
    {
        public void Assert<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            kb.Assert(rule);
        }
        public bool Contains<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            return kb.Contains(rule);
        }
        public void Retract<X>(Expression<Func<IKnowledge, X, bool>> consequent, params Expression<Func<IKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            kb.Retract(rule);
        }
        public IQueryable<X> Query<X>(params Expression<Func<IKnowledge, X, bool>>[] query)
        {
            var _query = Expression.Call(null, _Query.MakeGenericMethod(typeof(X)), query);
            return kb.Query(_query).Cast<X>();
        }
    }
}
```

### Rule Syntax Example

> [!NOTE]
> ```cs
> kb.Assert<(Person x, Person y, Person z)>((kb, v) => kb.UncleOf(v.y, v.z), (kb, v) => kb.FatherOf(v.x, v.z), (kb, v) => kb.BrotherOf(v.x, v.y));
> ```

### Query Syntax Example

> [!NOTE]
> ```cs
> Person alex = new Person("Alex Smith");
>
> kb.Query<(Person x, Person y)>((kb, v) => kb.BrotherOf(alex, v.x), (kb, v) => kb.FatherOf(v.x, v.y)).Select(v => v.y);
> ```

## Second-order Logic and Recursive Expressiveness

In the system considered above, special types for variables would include delegate types, which would enable second-order expressions, and lambda expressions, e.g., `Expression<Func<IKnowledge, bool>>`, which would enable a recursive expressiveness.

Here is an example of a second-order expression, a rule with a variable of a delegate type, a predicate:
```cs
kb.Assert<(Func<IKnowledge, Person, Person, bool> P, Person x, Person y)>(...).Build();
```

Here is an example of a predicate which could receive expressions as arguments:
```cs
[Predicate]
public static bool Meta(this IKnowledge kb, Expression<Func<bool>> x, Expression<Func<bool>> y)
{
    return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
}
```
