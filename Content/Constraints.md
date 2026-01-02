# Constraints

By utilizing static methods from the `System.Diagnostics.Contracts.Contract` static class, or from a new static class drawing inspiration from it, developers can create lambda expressions with logical meanings and use these expressions as constraints on objects, e.g., describing invariants, constraints on collections, or constraints on sequences or progressions.

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

    public IConstraintCollection GetCollection(LambdaExpression declaration);
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

    public IConstraintCollection<R> GetCollection<R>(Expression<Func<T, R>> declaration);
}

public interface IConstraint<T> : IConstraint
{
    public void Check(T value);

    public new Expression<Action<T>> Expression { get; }
}
```
