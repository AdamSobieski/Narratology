# Knowledge

Below, an approach is presented for representing and working with strongly-typed structured knowledge, propositional-logical expressions, rules, and queries in C#.

## Predicates

Predicates can be represented as static methods, receiving a number of strongly-typed inputs and returning expressions for functions which receive knowledgebases and return Boolean values.

```cs
namespace Example
{
    public static partial class Predicates
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

By means of the `using static` feature, developers can access their desired collections of predicates, easily adding them into a global scope or context. This would, for the predicates defined, above, resemble `using static Example.Predicates;`. In this way, developers can simply type `FatherOf`, `BrotherOf`, or `UncleOf` to access the predicates in C#.

## Knowledgebases

The following knowledgebase interfaces can simplify working with expressions, rules, and queries.

```cs
public interface IReadOnlyKnowledge
{
    bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    IQueryable Query(LambdaExpression[] query);

    IReadOnlyKnowledge Construct(KnowledgeConstructionOptions options);
}
```

```cs
public interface IKnowledge : IReadOnlyKnowledge
{
    void Assert(Expression<Func<IReadOnlyKnowledge, bool>> expression);

    void Retract(Expression<Func<IReadOnlyKnowledge, bool>> expression);
}
```

### Builtin Extension Methods

```cs
public static partial class Builtin
{
    extension(IReadOnlyKnowledge kb)
    {
        public IQueryable<X> Query<X>(params Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>>[] query)
        {
            return kb.Query(query).Cast<X>();
        }
    }
}
```

### Working with Expressions

```cs
kb.Assert(BrotherOf(alex, bob));

kb.Entails(BrotherOf(alex, bob));

kb.Retract(BrotherOf(alex, bob));
```

### Working with Rules

While extension methods can be provided for more syntactic sugar, a default technique for working with rules could resemble:

```cs
kb.Assert(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))));

kb.Contains(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))));

kb.Retract(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))));
```

### Working with Queries

```cs
kb.Query<(Person x, Person y)>(v => BrotherOf(alex, v.x), v => FatherOf(v.x, v.y)).Select(v => v.y);
```
```cs
kb.Query<(Person x, Person y)>().Where(v => BrotherOf(alex, v.x)).Where(v => FatherOf(v.x, v.y)).Select(v => v.y);
``` 
Note that using an extension method like `Where(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> functor)`, per the second example, above, might require that the `IReadOnlyKnowledge` interface's method `Query()` return an interface of type `IKnowledgeQueryable` / `IKnowledgeQueryable<X>` which extends `IQueryable` / `IQueryable<X>`.

### Variables for Predicates

One might want to be able to use variables for predicates.

```cs
kb.Assert(ForAll<Func<object, object, Expression<Func<IReadOnlyKnowledge, bool>>>>(P => ...));
```

### Variables for Expressions

One might want to be able to use variables for expressions.

```cs
kb.Assert(ForAll<Expression<Func<IReadOnlyKnowledge, bool>>>(expr => ...));
```

### Reification, Quoting, and Recursion

One approach to quoting expressions involves that a `Construct()` method on `IReadOnlyKnowledge` could receive a variable-length array of arguments of type expression, `Expression<Func<IReadOnlyKnowledge, bool>>`, a `KnowledgeConstructionOptions` options argument, and return an `IReadOnlyKnowledge` collection of expressions (where such collections could contain zero, one, or more expressions).

```cs
var content = kb.Construct(new KnowledgeConstructionOptions { Additions = [BrotherOf(bob, alex), BrotherOf(bob, charlie)] });
kb.Assert(AccordingTo(content, bob));
```

## Builtin Logical Predicates

### Rules

A builtin predicate can be provided, resembling Prolog's `:-` operator, called `Rule` here:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Rule(Expression<Func<IReadOnlyKnowledge, bool>> head, Expression<Func<IReadOnlyKnowledge, bool>> body)
{
    return kb => kb.Entails(Rule(head, body));
}
```

### `And`, `Or`, and `Not`

If `And`, `Or`, and `Not` are to be be provided as builtin predicates, they could resemble:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> And(Expression<Func<IReadOnlyKnowledge, bool>> left, Expression<Func<IReadOnlyKnowledge, bool>> right)
{
    return kb => kb.Entails(And(left, right));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Or(Expression<Func<IReadOnlyKnowledge, bool>> left, Expression<Func<IReadOnlyKnowledge, bool>> right)
{
    return kb => kb.Entails(Or(left, right));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Not(Expression<Func<IReadOnlyKnowledge, bool>> expression)
{
    return kb => kb.Entails(Not(expression));
}
```

The following predicates may also be useful:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsAll(Expression<Func<IReadOnlyKnowledge, bool>> left, Expression<Func<IReadOnlyKnowledge, bool>> right)
{
    return kb => kb.Entails(left) && kb.Entails(right);
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsAny(Expression<Func<IReadOnlyKnowledge, bool>> left, Expression<Func<IReadOnlyKnowledge, bool>> right)
{
    return kb => kb.Entails(left) || kb.Entails(right);
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> EntailsNone(Expression<Func<IReadOnlyKnowledge, bool>> expression)
{
    return kb => !kb.Entails(expression);
}
```

### Logical Quantification

With respect to quantification, builtin predicates for `Exists` and `ForAll` could resemble:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Exists<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> expression)
{
    return kb => kb.Entails(Exists<X>(expression));
}

[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> ForAll<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> expression)
{
    return kb => kb.Entails(ForAll<X>(expression));
}
```

### Lambda Calculus

With respect to lambda calculus, a builtin predicate for `Lambda` could resemble:

```cs
[Predicate]
public static Expression<Func<IReadOnlyKnowledge, bool>> Lambda<X>(Expression<Func<X, Expression<Func<IReadOnlyKnowledge, bool>>>> expression)
{
    return kb => kb.Entails(Lambda<X>(expression));
}
```

## Knowledgebase Overlays

If knowledgebases could function as overlays to other knowledgebases, knowledge-based objects could interact with their own small, mutable foreground knowledgebases while simultaneously benefitting from that reasoning possible as a result of using the many more expressions and rules available in a larger, immutable, background knowledgebase.

Overlays can be viewed as consisting of one set of expressions to add, and another set to remove, relative to a background knowledgebase. Overlays, then, could be provided to background knowledgebases' `Entails()` and `Query()` methods in the form of expressions intended to be added or removed before performing these operations.

```cs
public interface IReadOnlyKnowledge
{
    bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression, IReadOnlyCollection<Expression<Func<IReadOnlyKnowledge, bool>>>? additions = null, IReadOnlyCollection<Expression<Func<IReadOnlyKnowledge, bool>>>? removals = null);
    
    IQueryable Query(LambdaExpression[] query, IReadOnlyCollection<Expression<Func<IReadOnlyKnowledge, bool>>>? additions = null, IReadOnlyCollection<Expression<Func<IReadOnlyKnowledge, bool>>>? removals = null);

    IReadOnlyKnowledge Construct(KnowledgeConstructionOptions options);
}
```

Alternatively, developers could make use of an `IKnowledgeDifference` interface to indicate those expressions to be added and removed for an operation.

```cs
public interface IReadOnlyKnowledge
{
    bool Entails(Expression<Func<IReadOnlyKnowledge, bool>> expression, IKnowledgeDifference? difference = null);
    
    IQueryable Query(LambdaExpression[] query, IKnowledgeDifference? difference = null);

    IReadOnlyKnowledge Construct(KnowledgeConstructionOptions options);
}
```

Instead, creating overlays could be accomplished via the `KnowledgeConstructionOptions` argument provided to the `Construct()` method (which would provide a `IReadOnlyKnowledge` which could later be cast to `IKnowledge`). In this second approach, knowledge-based objects would only need to interact with their smaller, mutable, foreground knowledgebase instances which would encapsulate the detail that they were overlays to larger background knowledgebases.

## Intensional Sets and Set Algebra

Intensional sets are a good use case for knowledgebase overlays. Here is a preliminary sketch of intensional sets:

```cs
public class IntensionalSet<T>
{
    public IntensionalSet(ITemplate<IReadOnlyKnowledge> template, IReadOnlyKnowledge background)
    {
        var foreground = template.Invoke([this]);

        // ...
    }

    private IReadOnlyKnowledge kb;

    public bool Contains(T element)
    {
         return kb.Entails(ElementOf(this, element));
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

Brainstorming, perhaps a "set builder" technique could be developed resembling:

```cs
var s1 = Set.Create<int>().Where(x => IsEven(x)).Where(x => IsGreaterThan(x, 10)).Build(large_kb);
```
```cs
var s2 = Set.Create<Person>().Where(x => BrotherOf(alex, x)).Build(large_kb);
```

## Questions

<details>
<summary>Click here to toggle view of some open design questions.</summary>

1. Should `IReadOnlyKnowledge` be enumerable, provide `GetEnumerator()`, or provide `AsEnumerable()` and/or `AsQueryable()` methods?

2. Should `IKnowledge` provide developers with means to provide a map, mapping types to `IEqualityComparer` instances?
   1. If so, this could be an aspect of a `KnowledgeConstructionOptions` argument when generating knowledgebases and using `Construct()`.
   2. If not, should developers be able to provide these maps using an optional argument to a `Query()` method?

3. Should the `Construct()` method on `IReadOnlyKnowledge` instead be three methods: `Clone()`, `Overlay()`, and `Quote()`?

4. Should predicates, in addition to being static methods, be static extension methods, perhaps extending a shared type `Vocabulary`?

5. Should `IKnowledge` provide methods for loading sets of expressions and rules from resources?
   1. If not, could loading be an aspect of a `KnowledgeConstructionOptions` argument when generating knowledgebases and using `Construct()`.

6. Should an `Assert()` method on `IKnowledge` provide optional parameters for specifying attribution, provenance, and/or justification?

7. Are shapes, constraints, and/or other data validation features desired for knowledgebases?

8. Is obtaining differences or deltas between `IReadOnlyKnowledge` instances a feature desired by developers?

9. How can the initialization of knowledgebase instances be simplified?
   1. Perhaps developers could utilize an initializer which receives metadata categories and uses these one or more metadata categories to populate a knowledgebase instance with expressions and rules.
</details>

## Optimizations

<details>
<summary>Click here to toggle view of some computational performance optimization topics.</summary>
<br>

1. The following shows a faster, but less readable, technique for generating expressions for predicate invocations.

```cs
public static class Predicates
{
    static MethodInfo _Entails = typeof(IReadOnlyKnowledge).GetMethod("Entails")!;
    static MethodInfo? _FatherOf;
    static MethodInfo? _BrotherOf;
    static MethodInfo? _UncleOf;

    [Predicate]
    public static Expression<Func<IReadOnlyKnowledge, bool>> FatherOf(Person x, Person y)
    {
        var predicate = _FatherOf ??= (MethodInfo)MethodBase.GetCurrentMethod()!;

        var kb = Expression.Parameter(typeof(IReadOnlyKnowledge), "kb");
        return Expression.Lambda<Func<IReadOnlyKnowledge, bool>>(Expression.Call(kb, _Entails, Expression.Call(null, predicate, Expression.Constant(x), Expression.Constant(y))), kb);
    }

    [Predicate]
    public static Expression<Func<IReadOnlyKnowledge, bool>> BrotherOf(Person x, Person y)
    {
        var predicate = _BrotherOf ??= (MethodInfo)MethodBase.GetCurrentMethod()!;

        var kb = Expression.Parameter(typeof(IReadOnlyKnowledge), "kb");
        return Expression.Lambda<Func<IReadOnlyKnowledge, bool>>(Expression.Call(kb, _Entails, Expression.Call(null, predicate, Expression.Constant(x), Expression.Constant(y))), kb);
    }

    [Predicate]
    public static Expression<Func<IReadOnlyKnowledge, bool>> UncleOf(Person x, Person y)
    {
        var predicate = _UncleOf ??= (MethodInfo)MethodBase.GetCurrentMethod()!;

        var kb = Expression.Parameter(typeof(IReadOnlyKnowledge), "kb");
        return Expression.Lambda<Func<IReadOnlyKnowledge, bool>>(Expression.Call(kb, _Entails, Expression.Call(null, predicate, Expression.Constant(x), Expression.Constant(y))), kb);
    }
}    
```
</details>

## See Also

* [{log}](https://www.clpset.unipr.it/setlog.Home.html)
* [λProlog](https://www.lix.polytechnique.fr/Labo/Dale.Miller/lProlog/)
* [JSetL](https://www.clpset.unipr.it/jsetl/)
* [Programming with {Sets}](https://www.cs.nmsu.edu/~complog/sets/)
