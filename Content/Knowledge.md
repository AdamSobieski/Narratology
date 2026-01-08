# Knowledge

Here are some considered approaches for representing strongly-typed structured knowledge, propositional-logical expressions, rules, and queries.

## Predicates as Extension Methods of Knowledgebases

The following example shows how predicates can be represented as simple extension methods which extend a knowledgebase interface. This technique provides numerous benefits including simplifying organizing collections of predicates, using namespaces, in .NET assemblies. Developers could access their desired predicates, including with IntelliSense features, by using namespaces in source-code files.

```cs
public static partial class ExampleModule
{
    [Predicate]
    public static bool FatherOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool BrotherOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool UncleOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }
}
```

## Knowledgebase API

These knowledgebase interfaces, a work in progress, are designed to be general-purpose, enabling developers to work with rules and queries programmatically, using convenient and approachable C# techniques, while being simultaneously scalable for developers to be able to load and parse collections of rules from resources.

```cs
public interface IReadOnlyKnowledge
{
    public bool Contains(MethodBase predicate, object?[] arguments);

    public bool Entails(MethodBase predicate, object?[] arguments);

    public bool Contains(Expression rule);

    public IQueryable Query(Expression query);
}
```

```cs
public interface IKnowledge : IReadOnlyKnowledge
{
    public void Assert(MethodBase predicate, object?[] arguments);

    public void Retract(MethodBase predicate, object?[] arguments);

    public void Assert(Expression rule);

    public void Retract(Expression rule);
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

    public static void Rule<X>(Func<IReadOnlyKnowledge, X, bool> consequent, params Func<IReadOnlyKnowledge, X, bool>[] antecedent)
    {

    }
    public static void Query<X>(params Func<IReadOnlyKnowledge, X, bool>[] antecedent)
    {

    }

    extension(IReadOnlyKnowledge kb)
    {
        public bool Contains<X>(Expression<Func<IReadOnlyKnowledge, X, bool>> consequent, params Expression<Func<IReadOnlyKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            return kb.Contains(rule);
        }
        public IQueryable<X> Query<X>(params Expression<Func<IReadOnlyKnowledge, X, bool>>[] query)
        {
            var _query = Expression.Call(null, _Query.MakeGenericMethod(typeof(X)), query);
            return kb.Query(_query).Cast<X>();
        }
    }

    extension(IKnowledge kb)
    {
        public void Assert(Expression<Func<IReadOnlyKnowledge, bool>> lambda)
        {
            ...
        }
        public void Retract(Expression<Func<IReadOnlyKnowledge, bool>> lambda)
        {
            ...
        }

        public void Assert<X>(Expression<Func<IReadOnlyKnowledge, X, bool>> consequent, params Expression<Func<IReadOnlyKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            kb.Assert(rule);
        }
        public void Retract<X>(Expression<Func<IReadOnlyKnowledge, X, bool>> consequent, params Expression<Func<IReadOnlyKnowledge, X, bool>>[] antecedent)
        {
            var rule = Expression.Call(null, _Rule.MakeGenericMethod(typeof(X)), [consequent, .. antecedent]);
            kb.Retract(rule);
        }
    }
}
```

### Example: Expressing a Rule in C#

> [!NOTE]
> ```cs
> kb.Assert<(Person x, Person y, Person z)>((kb, v) => kb.UncleOf(v.y, v.z), (kb, v) => kb.FatherOf(v.x, v.z), (kb, v) => kb.BrotherOf(v.x, v.y));
> ```

### Example: Expressing a Query in C#

> [!NOTE]
> ```cs
> Person alex = new Person("Alex Smith");
>
> kb.Query<(Person x, Person y)>((kb, v) => kb.BrotherOf(alex, v.x), (kb, v) => kb.FatherOf(v.x, v.y)).Select(v => v.y);
> ```

## Second-order Logic

In addition to the system considered, above, variables could be delegate types; this would enable second-order expressions.

Here is an sketch of such a second-order expression, a rule with a predicate variable:
```cs
kb.Assert<(Func<IReadOnlyKnowledge, object, object, bool> P, object x, object y)>((kb, v) => v.P(kb, v.y, v.x), (kb, v) => kb.IsSymmetric(v.P), (kb, v) => v.P(kb, v.x, v.y));
```

## Reification, Quoting Expressions, and Recursion

A number of approaches are being explored involving: (1) reifying expressions, (2) quoting expressions, and (3) allowing expressions to be used as arguments in expressions, e.g.: `P1(x, P2(y, z))`.

With an `AccordingTo()` predicate resembling:

```cs
[Predicate]
public static bool AccordingTo(this IReadOnlyKnowledge kb, Expression expression, Person person)
{
    return kb.Entails(MethodBase.GetCurrentMethod()!, [expression, person]);
}
```

and a special builtin `Quote()` extension method on `IReadOnlyKnowledge`, the C# for quoting-related scenarios might resemble:

```cs
kb.Assert(k1 => k1.AccordingTo(k1.Quote(() => k1.BrotherOf(bob, alex)), bob));
```

## Attributes and Predicate Definitions

Predicates could use attributes to reference types having parameterless constructors and implementing an interface resembling:

```cs
public interface IPredicateDefinition
{
    public IReadOnlyKnowledge GetDefinition(MethodBase predicate, IReadOnlyKnowledge? callerMetadata = null);
}
```

Symmetric binary predicates, for example, could refer to a reusable type, e.g., `SymmetricPredicate`, which might use a "knowledgebase template" to instantiate a small, concrete, read-only knowledgebase when provided with that specific `MethodBase` having components of its definition requested. This small, concrete knowledgebase might contain only one expression, a unary expression indicating that the requested predicate was symmetric.

```cs
public static partial class ExampleModule
{
    [Predicate]
    [Definition(typeof(SymmetricPredicate))]
    public static bool BrotherOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails(MethodBase.GetCurrentMethod()!, [x, y]);
    }
}
```

When a knowledgebase encounters an unrecognized predicate, for instance `BrotherOf`, it could, configurably, choose to examine the predicate's method for one or more `DefinitionAttribute` attributes to use to create referenced types to request read-only knowledgebases containing components of the unrecognized predicate's definition.
