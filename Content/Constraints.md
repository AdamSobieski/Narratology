# Constraints

```cs
public interface IHasConstraints
{
    public IConstraintCollection Constraints { get; }
}

public interface IConstraintCollection : IEnumerable
{
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
