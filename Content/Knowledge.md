# Knowledge

Here are some approaches for representing strongly-typed structured knowledge, propositional-logical expressions, rules, and queries.

## Predicates as Static Extension Methods

Predicates can be represented as static extension methods on a type `Vocabulary`. By means of the `using static` feature, developers could access their desired collections of predicates, easily adding them into a global scope or context. This would, for the predicates defined, below, resemble: `using static Example.Predicates;`.

Using such techniques, collections of predicates, from multiple teams, in multiple .NET assemblies, could be organized and accessed &ndash; including compatibly with IntelliSense features.

```cs
namespace Example
{
    public static partial class Predicates
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
}
```

## Knowledgebase Interfaces

These knowledgebase interfaces, a work in progress, are designed to simplify working with expressions, rules, and queries.

```cs
public interface IReadOnlyKnowledge
{
    public bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    public bool ContainsRule(LambdaExpression consequent, LambdaExpression[] antecedent);

    public IQueryable Query(LambdaExpression[] query);

    public IReadOnlyKnowledge Create(Expression<Func<IReadOnlyKnowledge, bool>>[] content, KnowledgeCreationOptions? options = null);
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

```cs
kb.Assert(BrotherOf(alex, bob));
```
```cs
kb.Entails(BrotherOf(alex, bob));
```
```cs
kb.Retract(BrotherOf(alex, bob));
```

### Working with Rules

```cs
kb.AssertRule<(Person x, Person y, Person z)>(v => UncleOf(v.y, v.z), v => FatherOf(v.x, v.z), v => BrotherOf(v.x, v.y));
```
```cs
kb.ContainsRule<(Person x, Person y, Person z)>(v => UncleOf(v.y, v.z), v => FatherOf(v.x, v.z), v => BrotherOf(v.x, v.y));
```
```cs
kb.RetractRule<(Person x, Person y, Person z)>(v => UncleOf(v.y, v.z), v => FatherOf(v.x, v.z), v => BrotherOf(v.x, v.y));
```

### Working with Queries

```cs
kb.Query<(Person x, Person y)>(v => BrotherOf(alex, v.x), v => FatherOf(v.x, v.y)).Select(v => v.y);
```
```cs
kb.Query<(Person x, Person y)>().Where(v => BrotherOf(alex, v.x)).Where(v => FatherOf(v.x, v.y)).Select(v => v.y);
``` 
Note that using an extension method like `Where(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> functor)`, per the second example, above, might require that the `IReadOnlyKnowledge` interface's method `Query()` return an interface of type `IKnowledgeQueryable` / `IKnowledgeQueryable<X>` which extends `IQueryable` / `IQueryable<X>`.

## Variables for Predicates

One might want to be able to use variables for predicates.

```cs
kb.AssertRule<(Func<object, object, Expression<Func<IReadOnlyKnowledge, bool>>> P, object x, object y)>(v => v.P(v.y, v.x), v => IsSymmetric(v.P), v => v.P(v.x, v.y));
```

## Variables for Expressions

One might want to be able to use variables for expressions, variables of the type `Expression<Func<IReadOnlyKnowledge, bool>>`.

```cs
kb.AssertRule<(Expression<Func<IReadOnlyKnowledge, bool>> expr, Person x)>(v => ...);
```

## Variables for Sets of Expressions

One might want to be able to use variables for sets of expressions, variables of the type `IReadOnlyKnowledge`.

```cs
kb.AssertRule<(IReadOnlyKnowledge kb, Person x, Person y)>(v => ...);
```

## Reification, Quoting, and Recursion

Approaches are being explored with respect to: (1) reifying expressions, (2) quoting expressions, and (3) enabling expressions to be used as arguments in expressions, e.g.: `P1(x, P2(y, z))`.

One approach to quoting expressions involves that a `Create()` method on `IReadOnlyKnowledge` could receive a variable-length array of arguments of type expression, `Expression<Func<IReadOnlyKnowledge, bool>>`, and return an `IReadOnlyKnowledge` collection of expressions (where such collections could contain zero, one, or more expressions).

```cs
var content = kb.Create([BrotherOf(bob, alex), BrotherOf(bob, charlie)]);
kb.Assert(AccordingTo(content, bob));
```

## Logical Predicates

If `And`, `Or`, and `Not` are to be be provided as builtin predicates, they might resemble:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> And(Expression<Func<IReadOnlyKnowledge, bool>> expr1, Expression<Func<IReadOnlyKnowledge, bool>> expr2)
{
    return kb => kb.Entails(And(expr1, expr2));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Or(Expression<Func<IReadOnlyKnowledge, bool>> expr1, Expression<Func<IReadOnlyKnowledge, bool>> expr2)
{
    return kb => kb.Entails(Or(expr1, expr2));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Not(Expression<Func<IReadOnlyKnowledge, bool>> expr)
{
    return kb => kb.Entails(Not(expr));
}
```

The following predicates, too, could be useful:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsAll(Expression<Func<IReadOnlyKnowledge, bool>> expr1, Expression<Func<IReadOnlyKnowledge, bool>> expr2)
{
    return kb => kb.Entails(expr1) && kb.Entails(expr2);
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsAny(Expression<Func<IReadOnlyKnowledge, bool>> expr1, Expression<Func<IReadOnlyKnowledge, bool>> expr2)
{
    return kb => kb.Entails(expr1) || kb.Entails(expr2);
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsNone(Expression<Func<IReadOnlyKnowledge, bool>> expr)
{
    return kb => !kb.Entails(expr);
}
```

## Logical Quantification

With respect to quantification, builtin predicates for `Exists` and `ForAll` might resemble:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Exists<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> expr)
{
    return kb => kb.Entails(Exists<X>(expr));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> ForAll<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> expr)
{
    return kb => kb.Entails(ForAll<X>(expr));
}
```

As defined, logical quantifiers can be easily nested:

```cs
var expression = ForAll<Nation>(nation => Exists<City>(city => HasCapital(nation, city)));
```

## Knowledgebase Overlays

A means should be developed for smaller knowledgebases to function as overlays to larger background knowledgebases. Certain objects, then, could access their own small foreground knowledgebases while simultaneously benefitting from that reasoning possible as a result of using the many expressions and rules in a larger referenced background knowledgebase.

Creating an overlay could be made as simple to do as specifying this intent in a `KnowledgeCreationOptions` argument provided to a knowledgebase's `Create()` method.

## Intensional Sets and Set Algebra

Scenarios to explore include those involving predicates like `ElementOf()`, using one or more rules to express definitions of sets, and enabling set algebraic operations on these intensional sets.

Perhaps a new type, e.g., `IntensionalSet<T>`, could be of use, in these regards. This new type would receive a set of rules with the `ElementOf()` predicate in its consequent, or receive a template with which to produce such a set of rules, in its constructor.

The following is a preliminary sketch of these ideas:

```cs
public class IntensionalSet<T>
{
    public IntensionalSet(ITemplate<IReadOnlyKnowledge> template, IReadOnlyKnowledge background)
    {
        foreground_kb = template.Invoke([this]);
        background_kb = background;
        // ...
    }

    private IReadOnlyKnowledge foreground_kb;
    private IReadOnlyKnowledge background_kb;

    public bool Contains(T element)
    {
         foreground_kb.Entails(ElementOf(this, x));
    }

    public IntensionalSet<T> IntersectWith(IntensionalSet<T> other) { ... }

    public IntensionalSet<T> UnionWith(IntensionalSet<T> other) { ... }

    public IntensionalSet<T> ExceptWith(IntensionalSet<T> other) { ... }

    public IntensionalSet<T> SymmetricExceptWith(IntensionalSet<T> other) { ... }

    public bool IsSubsetOf(IntensionalSet<T> other) { ... }

    public bool IsSupersetOf(IntensionalSet<T> other) { ... }

    public bool IsProperSubsetOf(IntensionalSet<T> other) { ... }

    public bool IsProperSupersetOf(IntensionalSet<T> other) { ... }
}
```

Intensional sets could also provide collection-like methods such as `Add(T item)` and `Remove(T item)` which would operate set-algebraically on singleton sets formed from their arguments.

Intensional sets might each reference their own small collection of rules involving the `ElementOf()` predicate, to provide a set-definition function, while referencing other `IReadOnlyKnowledge` collections of many more expressions and rules, so that many intensional sets could be easily and efficiently created and worked with in .NET .

Brainstorming, a "set builder" technique could be developed, perhaps resembling:

```cs
var s1 = Set.Create<int>().Where(x => IsEven(x)).Where(x => IsGreaterThan(x, 10)).Build(large_kb);
```
```cs
var s2 = Set.Create<Person>().Where(x => BrotherOf(alex, x)).Build(large_kb);
```

## Attributes, Definitions and Metadata for Predicates

Developers could make use of attributes on predicates to reference reusable types of use for retrieving aspects of the predicates' definitions and metadata.

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

1. Should `IReadOnlyKnowledge` be enumerable, provide `GetEnumerator()`, or provide `AsEnumerable()` and/or `AsQueryable()` methods?

2. Should `IKnowledge` provide developers with means to provide a map, mapping types to `IEqualityComparer` instances?
   1. If so, this could be an aspect of a `KnowledgeCreationOptions` argument when generating knowledgebases and using `Create()`.
   2. If not, should developers be able to provide these maps using an optional argument to a `Query()` method?

3. Should rules utilize a builtin predicate which receives expressions as its arguments?
   1. If so, rules could describe rules as consequents.

4. Should rules be able to have rules as their consequents?

5. Should `IKnowledge` provide methods for loading sets of expressions and rules from resources?
   1. If not, could loading be an aspect of a `KnowledgeCreationOptions` argument when generating knowledgebases and using `Create()`.

6. Should an `Assert()` method on `IKnowledge` provide optional parameters for specifying attribution, provenance, and/or justification?

7. Are shapes, constraints, and/or other data validation features desired for knowledgebases?

8. Is obtaining differences or deltas between `IReadOnlyKnowledge` instances a feature desired by developers?

9. How can the initialization of knowledgebase instances be simplified?
   1. Perhaps developers could utilize an initializer which receives metadata categories and uses these one or more metadata categories to populate a knowledgebase instance with expressions and rules.

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
