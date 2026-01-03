# Constraints

## A Constraints System

Here are some interfaces for a constraints system using `System.Linq.Expressions` lambda expressions.

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

## Representing Invariants and Declarations

Invariants are constraints which must apply to their objects in all cases. Declarations are declared knowledge, stored in objects' constraint sets, about related objects. Invariants and declarations can both be represented using `System.Linq.Expressions` expression trees, more specifically method-call expressions.

As envisioned, lambda expressions can express method calls to special static methods including:

```cs
public static class Constraint
{
    public static void Declare<T, R>(T on, Func<T, R> map, Action<R> action)
    {

    }

    public static void Assert<T>(T on, Func<T, bool> predicate)
    {
        if (!predicate(on)) throw new ConstraintException("Assertion failed.");
    }
    public static void Assert<T>(T on, Func<T, bool> predicate, string message)
    {
        if (!predicate(on)) throw new ConstraintException(message);
    }

    public static void Invariant<T>(T on, Func<T, bool> predicate)
    {
        if (!predicate(on)) throw new ConstraintException("Invariant condition check failed.");
    }
    public static void Invariant<T>(T on, Func<T, bool> predicate, string message)
    {
        if (!predicate(on)) throw new ConstraintException(message);
    }

    public static void When<T>(T on, Func<T, bool> condition, Action<T> action)
    {
        try
        {
            if (!condition(on)) return;
            action(on);
        }
        catch (ConstraintException e)
        {
            throw new ConstraintException("Conditional assertion failed.", e);
        }
    }
    public static void When<T>(T on, Func<T, bool> condition, Action<T> action, string message)
    {
        try
        {
            if (!condition(on)) return;
            action(on);
        }
        catch (ConstraintException e)
        {
            throw new ConstraintException(message, e);
        }
    }
}
```

## Method Chaining, Fluent Interfaces, and Constraints

Here is a preliminary fluent interface for building constraints, `IConstraintCollectionBuilder<T>`:

```cs
public interface IConstraintCollectionBuilder<T>
{
    public IConstraintCollectionBuilder<T> Declare<R>(Expression<Func<T, R>> map, params Expression<Action<IConstraintCollectionBuilder<R>>>[] actions);

    public IConstraintCollectionBuilder<T> Assert(params Expression<Func<T, bool>>[] assertions);
    public IConstraintCollectionBuilder<T> Invariant(params Expression<Func<T, bool>>[] predicates);
    public IConstraintCollectionBuilder<T> When(Expression<Func<T, bool>> condition, params Expression<Action<IConstraintCollectionBuilder<T>>>[] actions);

    public IConstraintCollectionBuilder<T> Assert(Expression<Func<T, bool>> assertion, string message);
    public IConstraintCollectionBuilder<T> Invariant(Expression<Func<T, bool>> predicate, string message);
    public IConstraintCollectionBuilder<T> When(Expression<Func<T, bool>> condition, Expression<Action<IConstraintCollectionBuilder<T>>> action, string message);

    public IConstraintCollection<T> Build();

    public Expression<Action<T>> GetLambdaExpression();
}
```

Using such a constraints builder, automaton implementations could easily provide inspectable constraints about themselves, e.g., cardinality constraints regarding their sets of initial states, and declare constraints about all navigators which they might provide via their `GetNavigator()` methods, e.g., cardinality constraints on the sets of their current states and on the numbers of edges traversed to reach these.

Here is a first example of how those constraints could be expressed using a fluent syntax:

```cs
var constraints = Constraint.Builder<DeterministicAcceptor>().Invariant(x => x.Start.Count() == 1).Declare(x => x.GetNavigator(), b1 => b1.Invariant(x => x.Current.Count() == 1)).Declare(x => x.GetNavigator(), b1 => b1.Invariant(x => x.Edges.Count() == 1)).Build();
```

Here is a second, more succint, example:
```cs
var constraints = Constraint.Builder<DeterministicAcceptor>().Invariant(x => x.Start.Count() == 1).Declare(x => x.GetNavigator(), b1 => b1.Invariant(x => x.Current.Count() == 1), b2 => b2.Invariant(x => x.Edges.Count() == 1)).Build();
```

Here is a third, yet more succinct, example:
```cs
var constraints = Constraint.Builder<DeterministicAcceptor>().Invariant(x => x.Start.Count() == 1).Declare(x => x.GetNavigator(), b1 => b1.Invariant(x => x.Current.Count() == 1, x => x.Edges.Count() == 1)).Build();
```

Invariants and declarations, together, enable expressiveness for extension members about determinism, `bool IsDeterministic { get; }`, and for other verifiable properties of automata.

These concepts have been successfully prototyped.
