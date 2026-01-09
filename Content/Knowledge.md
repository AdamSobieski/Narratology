# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, propositional-logical expressions, rules, and queries.

## Predicates as Static Extension Methods

Predicates can be represented as static extension methods on a type `Vocabulary`. This technique can provide numerous benefits including simplifying organizing large collections of predicates, using namespaces, in one or more .NET assemblies. Developers could access their desired predicates, compatibly with IntelliSense features, by using namespaces in source-code files.

```cs
public static partial class ExampleModule
{
    extension(Vocabulary vocab)
    {
        [Predicate]
        public static Expression<Func<IReadOnlyKnowledge, bool>> FatherOf(Person x, Person y)
        {
            return kb => kb.Entails(FatherOf(x, y));
        }

        [Predicate]
        public static Expression<Func<IReadOnlyKnowledge, bool>> BrotherOf(Person x, Person y)
        {
            return kb => kb.Entails(BrotherOf(x, y));
        }

        [Predicate]
        public static Expression<Func<IReadOnlyKnowledge, bool>> UncleOf(Person x, Person y)
        {
            return kb => kb.Entails(UncleOf(x, y));
        }
    }
}
```

## Knowledgebase Interfaces

These knowledgebase interfaces, a work in progress, are designed to be general-purpose, enabling developers to work with rules and queries programmatically, using convenient and approachable C# techniques, while being simultaneously scalable for developers to be able to load and parse collections of expressions and rules from resources.

```cs
public interface IReadOnlyKnowledge
{
    public bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    public bool ContainsRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public IQueryable Query(LambdaExpression[] query);

    public IReadOnlyKnowledge Create(params Expression<Func<IReadOnlyKnowledge, bool>>[] contents);
}
```

```cs
public interface IKnowledge : IReadOnlyKnowledge
{
    public void Assert(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    public void Retract(Expression<Func<IReadOnlyKnowledge, bool>> expression);

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
        public bool ContainsRule<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> consequent, params Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>>[] antecedent)
        {
            return kb.ContainsRule(consequent, antecedent);
        }

        public IQueryable<X> Query<X>(params Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>>[] query)
        {
            if (query.Length == 0) return Enumerable.Empty<X>().AsQueryable();

            return kb.Query(query).Cast<X>();
        }
    }

    extension(IKnowledge kb)
    {
        public void AssertRule<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> consequent, params Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>>[] antecedent)
        {
            kb.AssertRule(consequent, antecedent);
        }

        public void RetractRule<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> consequent, params Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>>[] antecedent)
        {
            kb.RetractRule(consequent, antecedent);
        }
    }
}
```

### Working with Expressions

> [!NOTE]
> ```cs
> kb.Assert(Vocabulary.BrotherOf(alex, bob));
> ```
> ```cs
> kb.Entails(Vocabulary.BrotherOf(alex, bob));
> ```
> ```cs
> kb.Retract(Vocabulary.BrotherOf(alex, bob));
> ```

### Working with Rules

> [!NOTE]
> ```cs
> kb.AssertRule<(Person x, Person y, Person z)>(v => Vocabulary.UncleOf(v.y, v.z), v => Vocabulary.FatherOf(v.x, v.z), v => Vocabulary.BrotherOf(v.x, v.y));
> ```
> ```cs
> kb.ContainsRule<(Person x, Person y, Person z)>(v => Vocabulary.UncleOf(v.y, v.z), v => Vocabulary.FatherOf(v.x, v.z), v => Vocabulary.BrotherOf(v.x, v.y));
> ```
> ```cs
> kb.RetractRule<(Person x, Person y, Person z)>(v => Vocabulary.UncleOf(v.y, v.z), v => Vocabulary.FatherOf(v.x, v.z), v => Vocabulary.BrotherOf(v.x, v.y));
> ```

### Querying

> [!NOTE]
> ```cs
> kb.Query<(Person x, Person y)>(v => Vocabulary.BrotherOf(alex, v.x), v => Vocabulary.FatherOf(v.x, v.y)).Select(v => v.y);
> ```

## Reification, Quoting, and Recursion

A number of approaches are being explored to: (1) reify expressions, (2) quote expressions, and (3) allow expressions to be used as arguments in expressions, e.g.: `P1(x, P2(y, z))`.

One approach involves that a `Create()` method on `IReadOnlyKnowledge` could receive a variable-length array of arguments of type `Expression<Func<IReadOnlyKnowledge, bool>>` and return an `IReadOnlyKnowledge` collection of expressions (such collections could contain zero, one, or more expressions).

```cs
var content = kb.Create(Vocabulary.BrotherOf(bob, alex), Vocabulary.BrotherOf(bob, charlie));
kb.Assert(Vocabulary.AccordingTo(content, bob));
```

## Variables for Predicates

Here is a sketch of a second-order logical expression, a rule with a predicate variable:
```cs
kb.AssertRule<(Func<object, object, Expression<Func<IReadOnlyKnowledge, bool>>> P, object x, object y)>(v => v.P(v.y, v.x), v => Vocabulary.IsSymmetric(v.P), v => v.P(v.x, v.y));
```

## Variables for Sets of Expressions

One might also want to be able to use variables for sets of expressions, variables of the type `IReadOnlyKnowledge`.

```cs
kb.AssertRule<(IReadOnlyKnowledge KB, Person x, Person y)>(v => ...);
```

## Scenarios Involving Multiple Knowledgebases

Important scenarios to be explored in greater detail include those where multiple knowledgebases are desired to be worked with simulataneously and those where knowledgebases may contain references to other nested knowledgebases as can occur with reification and quoting.

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
public class DefinitionAttribute : Attribute
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

one could express the example predicates in a manner resembling:

```cs
[Predicate]
[Definition(typeof(Inverse), typeof(ExampleModule), nameof(SonOf), typeof(Person), typeof(Person))]
public static Expression<Func<IReadOnlyKnowledge, bool>> FatherOf(Person x, Person y)
{
    return kb => kb.Entails(FatherOf(x, y));
}

[Predicate]
[Definition(typeof(Inverse), typeof(ExampleModule), nameof(FatherOf), typeof(Person), typeof(Person))]
public static Expression<Func<IReadOnlyKnowledge, bool>> SonOf(Person x, Person y)
{
    return kb => kb.Entails(SonOf(x, y));
}

[Predicate]
[Definition(typeof(Symmetric))]
public static Expression<Func<IReadOnlyKnowledge, bool>> BrotherOf(Person x, Person y)
{
    return kb => kb.Entails(BrotherOf(x, y));
}
```

When a knowledgebase encounters an unrecognized predicate, it could opt to examine that predicate's `MethodInfo`'s custom attributes for one or more `DefinitionAttribute` attributes to make use of to create the referenced types, using their parameterless constructors, and then to provide arguments to these instances' `GetDefinition()` methods to request read-only knowledgebases containing aspects of the unrecognized predicate's definition.

## Questions

1. Should `IReadOnlyKnowledge` be enumerable or provide an `AsEnumerable()` and/or `AsQueryable()` method?

2. Should `IKnowledge` provide developers with means to provide `IEqualityComparer` instances for types?
   1. If not, might developers be able to provide these using an optional argument to a `Query()` method?

3. Should "and", "or", and 'not" be provided as builtin predicates?

4. Should rules use a builtin predicate which receives expressions as its arguments?
   1. If so, rules could have consquent expressions using the special predicate.

5. Should rules be able to have rules as their consequents?

6. Should `IReadOnlyKnowledge` provide methods for loading sets of expressions and rules from resources?

7. Should `Assert()` methods on `IKnowledge` include variants for providing attribution, provenance, and/or justifications?

8. Are "shapes", constraints, and/or other data validation features desired for knowledgebases?

9. Is obtaining differences or deltas between `IReadOnlyKnowledge` instances a feature desired by developers?

10. How should the knowledgebase interfaces, above, be compared and constrasted to alternatives, e.g., below, where sets of rules can receive interfaces to sets of expressions, as input, to produce interfaces to output sets of expressions?
    1. Above, rules can be added to and subtracted from collections which can contain both expressions and rules, on the fly.
    2. Below, sets of rules can process input expression sets to produce output expression sets.

<details>
<summary>Click here to toggle view of an alternative set of interfaces.</summary>
<br>

```cs
public interface IReadOnlyKnowledge
{
    public bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    public IQueryable Query(LambdaExpression[] query);

    public IReadOnlyKnowledge Create(params Expression<Func<IReadOnlyKnowledge, bool>>[] contents);
}

public interface IKnowledge : IReadOnlyKnowledge
{
    public void Assert(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    public void Retract(Expression<Func<IReadOnlyKnowledge, bool>> expression);
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
