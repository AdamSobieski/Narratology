# Knowledge

An approach is presented, below, for representing and working with strongly-typed structured knowledge, [propositional-logical](https://en.wikipedia.org/wiki/Propositional_logic) expressions, [fuzzy-logical](https://en.wikipedia.org/wiki/Fuzzy_logic) expressions, rules, queries, intensional sets, and more.

## Propositions

A generic type for propositions is sketched, below, extending `System.Linq.Expressions.Expression`.

The generic type `Proposition<bool>` is for Boolean propositions. The types `Proposition<float>` and `Proposition<double>` are for simple fuzzy propositions.

Using a generic struct, `ConfidenceValue<TConfidence, TValue>`, developers could create propositions of types `bool`, `float`, or `double` with an additional confidence score, e.g., `Proposition<ConfidenceValue<double, double>>`.

```cs
public sealed class Proposition<TEvaluate> : Expression
{
    internal Proposition(MethodInfo method, Expression[]? arguments)
    {
        m_expression = Expression.Call(method, arguments);
    }
    internal Proposition(Expression<Func<IEvaluator<TEvaluate>, TEvaluate>> lambda)
    {
        m_expression = null;
        m_lambda = lambda;
    }

    MethodCallExpression? m_expression;
    Expression<Func<IEvaluator<TEvaluate>, TEvaluate>>? m_lambda;

    public MethodInfo? Method => m_expression?.Method ?? null;

    public ReadOnlyCollection<Expression> Arguments
    {
        get
        {
            if (m_expression == null)
            {
                return ReadOnlyCollection<Expression>.Empty;
            }
            return m_expression.Arguments;
        }
    }

    public override ExpressionType NodeType => ExpressionType.Extension;

    public PropositionType PropositionType
    {
        get
        {
            if (m_expression == null) return PropositionType.Lambda;
            else return PropositionType.Call;
        }
    }

    public override Type Type => typeof(Func<IEvaluator<TEvaluate>, TEvaluate>);

    public override bool CanReduce => true;

    public override Expression Reduce()
    {
        if (m_lambda == null)
        {
            if (m_expression != null)
            {
                var type = typeof(IEvaluator<TEvaluate>);
                var evaluate = type.GetMethod(nameof(IEvaluator<TEvaluate>.Evaluate))!;

                var kb = Expression.Parameter(type, "kb");
                var call = Expression.Call(kb, evaluate, m_expression);

                m_lambda = Expression.Lambda<Func<IEvaluator<TEvaluate>, TEvaluate>>(call, kb);
            }
            else
            {
                throw new Exception();
            }
        }
        return m_lambda;
    }
}
```

Note that one can add static extension methods to the type `System.Linq.Expressions.Expression`.

```cs
public static partial class Extensions
{
    extension(Expression)
    {
        public static Proposition<TEvaluate> Proposition<TEvaluate>(MethodInfo method, params Expression[] arguments)
        {
            return new Proposition<TEvaluate>(method, arguments);
        }
        public static Proposition<TEvaluate> Proposition<TEvaluate>(Expression<Func<IEvaluator<TEvaluate>, TEvaluate>> lambda)
        {
            return new Proposition<TEvaluate>(lambda);
        }
    }
}
```

## Predicates

Predicates can be represented using static methods, receiving a number of strongly-typed input arguments and returning propositions.

```cs
public static partial class Module
{
    [Predicate]
    public static Proposition<bool> FatherOf(Person x, Person y)
    {
        return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(x), Expression.Constant(y)]);
    }

    [Predicate]
    public static Proposition<bool> BrotherOf(Person x, Person y)
    {
        return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(x), Expression.Constant(y)]);
    }

    [Predicate]
    public static Proposition<bool> UncleOf(Person x, Person y)
    {
        return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(x), Expression.Constant(y)]);
    }

    [Predicate]
    public static Proposition<ConfidenceValue<double, double>> LikesIceCream(Person x)
    {
        return Expression.Proposition<ConfidenceValue<double, double>>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(x)]);
    }
}
```

## Knowledgebases

The following generic knowledgebase interfaces simplify working with propositions, rules, and queries.

Note that a knowledgebase implementation could implement both `IKnowledge<bool>` and `IKnowledge<double>` interfaces.

```cs
public interface IEvaluator<TEvaluate>
{
    TEvaluate Evaluate(Proposition<TEvaluate> proposition);
}
```

```cs
public interface IReadOnlyKnowledge<TEvaluate> : IEvaluator<TEvaluate>
{
    IKnowledgeQueryable<TEvaluate, X> Query<X>(Expression<Func<X, Proposition<TEvaluate>>> query);

    IKnowledge<TEvaluate> Clone();

    IKnowledge<TEvaluate> Overlay();

    IReadOnlyKnowledge<TEvaluate> Quote(params Proposition<TEvaluate>[] propositions);
}
```

```cs
public interface IKnowledge<TEvaluate> : IReadOnlyKnowledge<TEvaluate>
{
    void Assert(Proposition<TEvaluate> proposition, TEvaluate value);

    void Retract(Proposition<TEvaluate> proposition);
}
```

### Working with Expressions

```cs
kb.Assert(BrotherOf(alex, bob), true);

kb.Evaluate(BrotherOf(alex, bob));

kb.Retract(BrotherOf(alex, bob));
```

### Working with Rules

While extension methods could be provided for more syntactic sugar, a default technique for working with rules could resemble:

```cs
kb.Assert(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))), true);

kb.Evaluate(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))));

kb.Retract(ForAll<Person>(x => ForAll<Person>(y => ForAll<Person>(z => Rule(UncleOf(y, z), And(FatherOf(x, z), BrotherOf(x, y)))))));
```

### Working with Queries

```cs
kb.Query<(Person x, Person y)>(v => BrotherOf(alex, v.x), v => FatherOf(v.x, v.y)).Select(v => v.y);

kb.Query<(Person x, Person y)>().Where(v => BrotherOf(alex, v.x)).Where(v => FatherOf(v.x, v.y)).Select(v => v.y);
``` 

### Variables for Predicates

One might want to be able to use variables for predicates.

```cs
kb.Assert(ForAll<Func<object, object, Proposition<bool>>>(P => ...), true);
```

### Variables for Propositions

One might want to be able to use variables for expressions.

```cs
kb.Assert(ForAll<Proposition<bool>>(expr => ...), true);
```

### Reification, Quoting, and Recursion

```cs
var content = kb.Quote([BrotherOf(bob, alex), BrotherOf(bob, charlie)]);
kb.Assert(AccordingTo(content, bob), true);
```

## Builtin Logical Predicates

### Rules

A builtin predicate can be provided, resembling Prolog's `:-` operator, called `Rule` here:

```cs
[Predicate]
public static Proposition<bool> Rule(Proposition<bool> head, Proposition<bool> body)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(head), Expression.Constant(body)]);
}
```

### `And`, `Or`, and `Not`

If `And`, `Or`, and `Not` are to be be provided as builtin predicates, they might resemble:

```cs
[Predicate]
public static Proposition<bool> And(Proposition<bool> left, Proposition<bool> right)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(left), Expression.Constant(right)]);
}

[Predicate]
public static Proposition<bool> Or(Proposition<bool> left, Proposition<bool> right)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(left), Expression.Constant(right)]);
}

[Predicate]
public static Proposition<bool> Not(Proposition<bool> expression)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Constant(expression)]);
}
```

The following predicates may also be useful:

```cs
[Predicate]
public static Proposition<bool> EntailsAll(Proposition<bool> left, Proposition<bool> right)
{
    return Expression.Proposition<bool>(kb => kb.Entails(left) && kb.Entails(right));
}

[Predicate]
public static Proposition<bool> EntailsAny(Proposition<bool> left, Proposition<bool> right)
{
    return Expression.Proposition<bool>(kb => kb.Entails(left) || kb.Entails(right));
}

[Predicate]
public static Proposition<bool> EntailsNone(Proposition<bool> expression)
{
    return Expression.Proposition<bool>(kb => !kb.Entails(expression));
}
```

### Logical Quantification

With respect to quantification, builtin predicates for `Exists` and `ForAll` could resemble:

```cs
[Predicate]
public static Proposition<bool> Exists<X>(Expression<Func<X, Proposition<bool>>> expression)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Quote(expression)]);
}

[Predicate]
public static Proposition<bool> ForAll<X>(Expression<Func<X, Proposition<bool>>> expression)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Quote(expression)]);
}
```

### Lambda Calculus

With respect to lambda calculus, a builtin predicate for `Lambda` could resemble:

```cs
[Predicate]
public static Proposition<bool> Lambda<X>(Expression<Func<X, Proposition<bool>>> expression)
{
    return Expression.Proposition<bool>((MethodInfo)MethodBase.GetCurrentMethod()!, [Expression.Quote(expression)]);
}
```

## Overlays

Knowledgebases could function as overlays to other larger knowledgebases. Knowledge-based objects could interact with their own small, mutable foreground knowledgebases while simultaneously benefitting from that reasoning possible as a result of using the many more expressions and rules available in a larger, immutable, background knowledgebase.

## Discussion

### Intensional Sets

Intensional sets are a good use case for knowledgebase overlays. Here is a preliminary sketch of intensional sets:

```cs
public class IntensionalSet<T>
{
    public IntensionalSet(ITemplate<IReadOnlyKnowledge<bool>> template, IReadOnlyKnowledge<bool> background)
    {
        var foreground = template.Invoke([this]);

        kb = background.Overlay();

        // ...
    }

    private IKnowledge<bool> kb;

    public bool Contains(T element)
    {
         return kb.Evaluate(ElementOf(this, element));
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

Intensional sets might each reference their own small collection of rules involving the `ElementOf()` predicate, to provide a set-definition function, while referencing other `IReadOnlyKnowledge<bool>` collections of many more expressions and rules, so that many intensional sets could be easily and efficiently created and worked with in .NET .

Brainstorming, perhaps a "set builder" technique could be developed resembling:

```cs
var s1 = Set.Create<int>().Where(x => IsEven(x)).Where(x => IsGreaterThan(x, 10)).Build(large_kb);
```
```cs
var s2 = Set.Create<Person>().Where(x => BrotherOf(alex, x)).Build(large_kb);
```

See Also: [{log}](https://www.clpset.unipr.it/setlog.Home.html), [λProlog](https://www.lix.polytechnique.fr/Labo/Dale.Miller/lProlog/), [JSetL](https://www.clpset.unipr.it/jsetl/), [Programming with {Sets}](https://www.cs.nmsu.edu/~complog/sets/).

### Concepts
See Also: [_Concepts_](/Content/Concepts.md)

It will be convenient for developers to be able to simply create and work with intensional sets.

Similarly, developers would also enjoy being able to create and work with ad-hoc "concepts". Intensional sets resemble the [classical theory](https://en.wikipedia.org/wiki/Concept#Classical_theory) of concepts and [definitionism](https://en.wikipedia.org/wiki/Definitionism). There are other theories of concepts to consider, e.g., the [prototype theory](https://en.wikipedia.org/wiki/Concept#Prototype_theory) and [theory-theory](https://en.wikipedia.org/wiki/Concept#Theory-theory).

## Questions

<details>
<summary>Click here to toggle view of some open design questions.</summary>

1. Should `IReadOnlyKnowledge<>` be enumerable, provide `GetEnumerator()`, or provide `AsEnumerable()` and/or `AsQueryable()` methods?

2. Should `IKnowledge<>` provide developers with means to provide a map, mapping types to `IEqualityComparer` instances?
   1. If not, should developers be able to provide these maps using an optional argument to a `Query()` method?

3. Should predicates, in addition to being static methods, be static extension methods, perhaps extending a shared type `Vocabulary`?

4. Should `IKnowledge<>` provide methods for loading sets of expressions and rules from resources?

5. Should an `Assert()` method on `IKnowledge<>` provide optional parameters for specifying attribution, provenance, and/or justification?

6. Are shapes, constraints, and/or other data validation features desired for knowledgebases?

7. Is obtaining differences or deltas between `IReadOnlyKnowledge<>` instances a feature desired by developers?

8. How can the initialization of knowledgebase instances be simplified?
   1. Perhaps developers could utilize an initializer which receives metadata categories and uses these one or more metadata categories to populate a knowledgebase instance with expressions and rules.

</details>
