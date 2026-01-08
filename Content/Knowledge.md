# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, propositional-logical expressions, rules, and queries.

## Predicates as Extension Methods of Knowledgebases

Predicates can be represented as simple extension methods which extend a knowledgebase interface. This technique provides numerous benefits including simplifying organizing collections of predicates, using namespaces, in .NET assemblies. Developers could access their desired predicates, including with IntelliSense features, by using namespaces in source-code files.

```cs
public static partial class ExampleModule
{
    [Predicate]
    public static bool FatherOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool BrotherOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
    }

    [Predicate]
    public static bool UncleOf(this IReadOnlyKnowledge kb, Person x, Person y)
    {
        return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
    }
}
```

## Knowledgebase Interfaces

These knowledgebase interfaces, a work in progress, are designed to be general-purpose, enabling developers to work with rules and queries programmatically, using convenient and approachable C# techniques, while being simultaneously scalable for developers to be able to load and parse collections of rules from resources.

```cs
public interface IReadOnlyKnowledge
{
    public bool Contains(MethodInfo predicate, object?[] arguments);

    public bool Entails(MethodInfo predicate, object?[] arguments);

    public bool ContainsRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public IQueryable Query(LambdaExpression[] query);

    public IReadOnlyKnowledge Quote(params Expression<Func<bool>>[] contents);
}
```

```cs
public interface IKnowledge : IReadOnlyKnowledge
{
    public void Assert(MethodInfo predicate, object?[] arguments);

    public void Retract(MethodInfo predicate, object?[] arguments);

    public void AssertRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public void RetractRule(LambdaExpression consequent, LambdaExpression[] antecedent);
}
```

### Builtin Extension Methods

The following builtins and extension methods are to provide developers with convenient and approachable syntax options for working with expressions, rules, and queries.

```cs
public static partial class Builtin
{
    extension(IReadOnlyKnowledge kb)
    {
        public void Contains(Expression<Func<bool>> expression)
        {
            ...
        }
        public void Entails(Expression<Func<bool>> expression)
        {
            ...
        }

        public bool ContainsRule<X>(Expression<Func<X, bool>> consequent, params Expression<Func<X, bool>>[] antecedent)
        {
            return kb.ContainsRule(consequent, antecedent);
        }
        public IQueryable<X> Query<X>(params Expression<Func<X, bool>>[] query)
        {
            if (query.Length == 0) return Enumerable.Empty<X>().AsQueryable();

            return kb.Query(query).Cast<X>();
        }
    }

    extension(IKnowledge kb)
    {
        public void Assert(Expression<Func<bool>> expression)
        {
            ...
        }
        public void Retract(Expression<Func<bool>> expression)
        {
            ...
        }

        public void AssertRule<X>(Expression<Func<X, bool>> consequent, params Expression<Func<X, bool>>[] antecedent)
        {
            kb.AssertRule(consequent, antecedent);
        }
        public void RetractRule<X>(Expression<Func<X, bool>> consequent, params Expression<Func<X, bool>>[] antecedent)
        {
            kb.RetractRule(consequent, antecedent);
        }

        public void Assert<T1>(Func<IReadOnlyKnowledge, T1, bool> predicate, T1 arg1)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Assert(predicate.Method, [arg1]);
        }
        public void Assert<T1, T2>(Func<IReadOnlyKnowledge, T1, T2, bool> predicate, T1 arg1, T2 arg2)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Assert(predicate.Method, [arg1, arg2]);
        }
        public void Assert<T1, T2, T3>(Func<IReadOnlyKnowledge, T1, T2, T3, bool> predicate, T1 arg1, T2 arg2, T3 arg3)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Assert(predicate.Method, [arg1, arg2, arg3]);
        }
        public void Assert<T1, T2, T3, T4>(Func<IReadOnlyKnowledge, T1, T2, T3, T4, bool> predicate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Assert(predicate.Method, [arg1, arg2, arg3, arg4]);
        }
        // ...

        public void Retract<T1>(Func<IReadOnlyKnowledge, T1, bool> predicate, T1 arg1)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Retract(predicate.Method, [arg1]);
        }
        public void Retract<T1, T2>(Func<IReadOnlyKnowledge, T1, T2, bool> predicate, T1 arg1, T2 arg2)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Retract(predicate.Method, [arg1, arg2]);
        }
        public void Retract<T1, T2, T3>(Func<IReadOnlyKnowledge, T1, T2, T3, bool> predicate, T1 arg1, T2 arg2, T3 arg3)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Retract(predicate.Method, [arg1, arg2, arg3]);
        }
        public void Retract<T1, T2, T3, T4>(Func<IReadOnlyKnowledge, T1, T2, T3, T4, bool> predicate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (predicate.Target != null) throw new ArgumentException();
            if (!predicate.Method.IsStatic) throw new ArgumentException();

            kb.Retract(predicate.Method, [arg1, arg2, arg3, arg4]);
        }
        // ...
    }
}
```

### Working with Expressions (Technique #1)

> [!NOTE]
> ```cs
> kb.Assert(ExampleModule.BrotherOf, alex, bob);
> ```
> ```cs
> kb.Entails(ExampleModule.BrotherOf, alex, bob);
> ```
> ```cs
> kb.Retract(ExampleModule.BrotherOf, alex, bob);
> ```

### Working with Expressions (Technique #2)

> [!NOTE]
> ```cs
> kb.Assert(() => kb.BrotherOf(alex, bob));
> ```
> ```cs
> kb.Entails(() => kb.BrotherOf(alex, bob));
> ```
> ```cs
> kb.Retract(() => kb.BrotherOf(alex, bob));
> ```

### Working with Rules

> [!NOTE]
> ```cs
> kb.AssertRule<(Person x, Person y, Person z)>(v => kb.UncleOf(v.y, v.z), v => kb.FatherOf(v.x, v.z), v => kb.BrotherOf(v.x, v.y));
> ```
> ```cs
> kb.ContainsRule<(Person x, Person y, Person z)>(v => kb.UncleOf(v.y, v.z), v => kb.FatherOf(v.x, v.z), v => kb.BrotherOf(v.x, v.y));
> ```
> ```cs
> kb.RetractRule<(Person x, Person y, Person z)>(v => kb.UncleOf(v.y, v.z), v => kb.FatherOf(v.x, v.z), v => kb.BrotherOf(v.x, v.y));
> ```

### Querying

> [!NOTE]
> ```cs
> kb.Query<(Person x, Person y)>(v => kb.BrotherOf(alex, v.x), v => kb.FatherOf(v.x, v.y)).Select(v => v.y);
> ```

## Reification, Quoting, and Recursion

A number of approaches are being explored to: (1) reify expressions, (2) quote expressions, and (3) allow expressions to be used as arguments in expressions, e.g.: `P1(x, P2(y, z))`.

One approach involves that a `Quote()` method on `IReadOnlyKnowledge` could receive a variable-length array of arguments of type `Expression<Func<bool>>` and return an `IReadOnlyKnowledge` collection of expressions (such collections could contain zero, one, or more expressions).

```cs
[Predicate]
public static bool AccordingTo(this IReadOnlyKnowledge kb, IReadOnlyKnowledge content, Person person)
{
    return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [content, person]);
}
```
```cs
var content = kb.Quote(() => kb.BrotherOf(bob, alex), () => kb.BrotherOf(bob, charlie));
kb.Assert(() => kb.AccordingTo(content, bob));
```

## Variables for Predicates

In addition to the system considered, above, variables could be delegate types; this would enable second-order logical expressions.

Here is an sketch of such a second-order logical expression, a rule with a predicate variable:
```cs
kb.AssertRule<(Func<IReadOnlyKnowledge, object, object, bool> P, object x, object y)>(v => v.P(kb, v.y, v.x), v => kb.IsSymmetric(v.P), v => v.P(kb, v.x, v.y));
```

## Variables for Sets of Expressions

In addition to creating rules and expressions about specific sets of expressions, `kb` above, one might want to use variables for sets of expressions, variables of the type `IReadOnlyKnowledge`.

```cs
kb.AssertRule<(IReadOnlyKnowledge KB, Person x, Person y)>(v => v.KB.BrotherOf(v.x, v.y) ...);
```

## Attributes and Predicate Definitions

Developers could make use of attributes on predicates to reference reusable types of use for retrieving aspects of the predicates' definitions.

```cs
public interface IPredicateDefinition
{
    public IReadOnlyKnowledge GetDefinition(MethodInfo predicate, object?[] args, IReadOnlyKnowledge? callerMetadata = null);
}
```

Using a new attribute:

```cs
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class DefinitionAttribute : Attribute
{
    public DefinitionAttribute(Type type, params object?[] args)
    {
        Type = type;
        Arguments = args;
    }

    public Type Type { get; }

    public object?[] Arguments { get; }
}
```

one could declare the example predicates in a manner resembling:

```cs
[Predicate]
[Definition(typeof(InverseDefinition), typeof(ExampleModule), nameof(SonOf), typeof(Person), typeof(Person))]
public static bool FatherOf(this IReadOnlyKnowledge kb, Person x, Person y)
{
    return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
}

[Predicate]
[Definition(typeof(InverseDefinition), typeof(ExampleModule), nameof(FatherOf), typeof(Person), typeof(Person))]
public static bool SonOf(this IReadOnlyKnowledge kb, Person x, Person y)
{
    return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
}

[Predicate]
[Definition(typeof(SymmetricDefinition))]
public static bool BrotherOf(this IReadOnlyKnowledge kb, Person x, Person y)
{
    return kb.Entails((MethodInfo)MethodBase.GetCurrentMethod()!, [x, y]);
}
```

When a knowledgebase encounters an unrecognized predicate, it could opt to examine that predicate's `MethodInfo`'s custom attributes for one or more `DefinitionAttribute` attributes to make use of to create the referenced types, using their parameterless constructors, and then to provide arguments to these instances' `GetDefinition()` methods to request read-only knowledgebases containing aspects of the unrecognized predicate's definition.

## Questions

1. Should `IReadOnlyKnowledge` be enumerable or provide an `AsEnumerable()` and/or `AsQueryable()` method?

2. Should `IKnowledge` provide developers with means to provide `IEqualityComparer` instances for types?
   1. If not, might developers be able to provide these using an argument to the `Query()` method?

3. Should the API for rules use a special (builtin) predicate that receives expressions as its arguments?
   1. If so, rules could have consquent expressions using the special predicate. Should rules be able to express rule consequents?

4. Should `IReadOnlyKnowledge` provide methods for loading sets of expressions and rules from resources?

5. Should `Assert()` methods on `IKnowledge` include variants for providing attribution, provenance, and/or justifications?

6. Are "shapes", constraints, and/or other data validation features desired for knowledgebases?

7. Is obtaining differences or deltas between `IReadOnlyKnowledge` instances a feature desired by developers?

8. How should the knowledgebase interfaces, above, be compared and constrasted to alternatives, e.g., below, where sets of rules can receive interfaces to sets of expressions, as input, to produce interfaces to output sets of expressions.
   1. Above, rules can be added to and subtracted from collections which can contain both expressions and rules, on the fly.
   2. Below, sets of rules can process input expression sets to produce output expression sets.

<details>
<summary>Click here to toggle view of an alternative set of interfaces.</summary>
<br>

```cs
public interface IReadOnlyKnowledge
{
    public bool Contains(MethodInfo predicate, object?[] arguments);

    public bool Entails(MethodInfo predicate, object?[] arguments);

    public IQueryable Query(LambdaExpression[] query);

    public IReadOnlyKnowledge Quote(params Expression<Func<bool>>[] contents);
}

public interface IKnowledge : IReadOnlyKnowledge
{
    public void Assert(MethodInfo predicate, object?[] arguments);

    public void Retract(MethodInfo predicate, object?[] arguments);
}

public interface IReadOnlyRuleSet
{
    public bool ContainsRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public IReadOnlyKnowledge Process(IReadOnlyKnowledge input);
}

public interface IRuleSet : IReadOnlyRuleSet
{
    public void AssertRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public void RetractRule(LambdaExpression consequent, LambdaExpression[] antecedent);
}
```
</details>
