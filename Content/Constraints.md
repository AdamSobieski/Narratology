# Constraints

By using static methods from the `System.Diagnostics.Contracts.Contract` static class, or static methods from a new static class drawing inspiration from it, developers could easily create `System.Linq.Expressions` expressions describing invariants, constraints on collections, and constraints on sequences or progressions.

```cs
public interface IHasConstraints
{
    public IConstraintCollection Constraints { get; }
}

public interface IConstraintCollection : IEnumerable
{
    public Type ParameterType { get; }

    public void Check(object value);

    public IEnumerable<LambdaExpression> Declarations { get; }

    public IConstraintCollection GetCollection(LambdaExpression map);
}

public interface IConstraint
{
    public void Check(object value);

    public LambdaExpression Expression { get; }
}
```
```cs
public interface IHasConstraints<T> : IHasConstraints
{
    public new IConstraintCollection<T> Constraints { get; }
}

public interface IConstraintCollection<T> : IConstraintCollection, IEnumerable<IConstraint<T>>
{
    public void Check(T value);

    public IConstraintCollection<R> GetCollection<R>(Expression<Func<T, R>> map);
}

public interface IConstraint<T> : IConstraint
{
    public void Check(T value);

    public new Expression<Action<T>> Expression { get; }
}
```

## Representing Invariants and Declarations using Lambda Expressions

Invariants are constraints which must apply to their objects in all cases. Declarations are declared knowledge, stored in objects' constraint sets, about related objects.

Invariants and declarations can both be represented using `System.Linq.Expressions` expression trees, more specifically as method-call expressions. When compiled, invariants throw exceptions when they are not applicable to checked objects; declarations, on the other hand, do nothing when compiled, but can be extracted from lambda expressions to become invariants for other objects.

For example, an automaton can provide inspectable constraints about itself, e.g., cardinality constraints regarding its set of initial states, and declare constraints about all navigators that it might provide via its `GetNavigator()` method, e.g., cardinality constraints on the sets of current states and on the numbers of edges traversed to reach them.

Lambda expressions can express method calls to static methods including:

```cs
public static class Constraint
{
    public static void Declare<T, R>(T on, Func<T, R> map, Action<R> action)
    {
        
    }
    public static void Invariant<T>(T on, Func<T, bool> predicate)
    {
        if (!predicate(on)) throw new Exception("Assertion failed.");
    }
    public static void Invariant<T>(T on, Func<T, bool> predicate, string message)
    {
        if (!predicate(on)) throw new Exception(message);
    }
}
```

Lambda expressions representing sequences of calls to meaningful static methods can be processed and reasoned upon as being sets of constraints, sets also containing declarations about objects related to those objects, e.g., automata describing conditions which hold for all of their navigators.

Invariants and declarations, together, enable the expressiveness for extension members about determinism, `bool IsDeterministic { get; }`, and for other verifiable properties of automata.

## Constraints on Collections

_Coming soon._

## Constraints on Navigations and Progressions

_Coming soon._
