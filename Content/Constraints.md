# Constraints

## A Constraints System

Here are some interfaces for a new constraints system using the `System.Linq.Expressions` model for lambda expressions.

```cs
public interface IHasConstraints
{
    public IConstraints Constraints { get; }
}

public interface IConstraints
{
    public Type ParameterType { get; }

    public void Check(object value);

    public IEnumerable<(LambdaExpression Map, LambdaExpression Create)> GetPromotionKeys();

    public IEnumerable<IAssertion> Assertions { get; }

    public IEnumerable<IConditionalAssertion> ConditionalAssertions { get; }

    public IEnumerable<IDeclaration> Declarations { get; }

    public IConstraints PromoteMatchingDeclarations(LambdaExpression map);

    public IConstraints PromoteMatchingDeclarations(LambdaExpression map, LambdaExpression create);
}

public interface IAssertion
{
    public LambdaExpression Expression { get; }

    public void Check(object value);
}

public interface IConditionalAssertion : IAssertion
{
    public LambdaExpression Condition { get; }

    public LambdaExpression Assertion { get; }
}

public interface IDeclaration
{
    public LambdaExpression Expression { get; }

    public LambdaExpression Map { get; }

    public LambdaExpression Create { get; }

    public object? Promote(LambdaExpression map, LambdaExpression create);
}
```
```cs
public interface IHasConstraints<T> : IHasConstraints
{
    public new IConstraints<T> Constraints { get; }
}

public interface IConstraints<T> : IConstraints
{
    public void Check(T value);

    public new IEnumerable<IAssertion<T>> Assertions { get; }

    public new IEnumerable<IConditionalAssertion<T>> ConditionalAssertions { get; }

    public new IEnumerable<IDeclaration<T>> Declarations { get; }

    public IConstraints<U> PromoteMatchingDeclarations<U>(Expression<Func<T, U>> map);

    public IConstraints<V> PromoteMatchingDeclarations<U, V>(Expression<Func<T, U>> map, Expression<Func<T, U, V>> create);
}

public interface IAssertion<T> : IAssertion
{
    public void Check(T value);

    public new Expression<Action<T>> Expression { get; }
}

public interface IConditionalAssertion<T> : IConditionalAssertion, IAssertion<T>
{
    public new Expression<Func<T, bool>> Condition { get; }

    public new Expression<Action<T>> Assertion { get; }
}

public interface IDeclaration<T> : IDeclaration
{
    public new Expression<Action<T>> Expression { get; }

    public object? Promote<U, V>(Expression<Func<T, U>> map, Expression<Func<T, U, V>> create);
}
```

## Representing Invariants and Declarations

Invariants are constraints which must apply to their objects in all cases. Declarations are declared knowledge, stored in objects' constraint sets, about related objects.

As envisioned, lambda expressions can express method calls to special static methods such as:

```cs
public static class Constraint
{
    public static void Declare<T, U>(T on, Func<T, U> map, Action<U> action)
    {

    }
    public static void Declare<T, U, V>(T on, Func<T, U> map, Func<T, U, V> create, Action<V> action)
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

## Method Chaining, Fluent Interfaces, and Constraint Building

Here is a preliminary fluent interface for building constraints, `IConstraintsBuilder<T>`:

```cs
public interface IConstraintsBuilder<T>
{
    public IConstraintsBuilder<T> Declare<U>(Expression<Func<T, U>> map, Expression<Action<IConstraintsBuilder<U>>> action);

    public IConstraintsBuilder<T> Declare<U, V>(Expression<Func<T, U>> map, Expression<Func<T, U, V>> create, Expression<Action<IConstraintsBuilder<V>>> action);

    public IConstraintsBuilder<T> Assert(Expression<Func<T, bool>> assertion, string? message = null);

    public IConstraintsBuilder<T> Invariant(Expression<Func<T, bool>> predicate, string? message = null);

    public IConstraintsBuilder<T> When(Expression<Func<T, bool>> condition, Expression<Action<IConstraintsBuilder<T>>> action, string? message = null);

    public IConstraints<T> Build();

    public Expression<Action<T>> GetLambdaExpression();
}
```

Using such a constraints builder, automaton implementations could easily provide inspectable constraints about themselves, e.g., cardinality constraints regarding their sets of initial states, and declare constraints about all navigators which they might provide via their `GetNavigator()` methods, e.g., cardinality constraints on the sets of their current states and on the numbers of edges traversed to reach these.

Here is an example of how constraints can be built using a fluent syntax enabled by `IConstraintsBuilder<T>`:

```cs
var constraints = Constraint.Builder<DeterministicAcceptor>()
    .Invariant(x => x.Start.Count() == 1)
    .Declare(x => x.GetNavigator(), b => b.Invariant(x => x.Current.Count() == 1))
    .Declare(x => x.GetNavigator(), b => b.Invariant(x => x.Edges.Count() == 1))
    .Build();
```

Invariants and declarations, together, enable expressiveness for extension members about determinism, `bool IsDeterministic { get; }`, and for other verifiable properties of automata.

A first version of these concepts was successfully prototyped. A second version is actively being developed.
