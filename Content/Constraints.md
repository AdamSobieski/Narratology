# Constraints

```cs
public interface IHasConstraints
{
    public IConstraintCollection Constraints { get; }
}

public interface IConstraintCollection : IEnumerable<IConstraint>
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
