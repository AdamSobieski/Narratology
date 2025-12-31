# Constraints

```cs
public interface IHasConstraints
{
    public IConstraintCollection Constraints { get; }
}

public interface IConstraintCollection : IEnumerable<IConstraint>
{
    public void Check(object value);

    public IEnumerable<LambdaExpression> Keys { get; }
    public IEnumerable<IConstraintCollection> Values { get; }

    public IConstraintCollection GetValue(LambdaExpression key);
}

public interface IConstraint
{
    public LambdaExpression Expression { get; }
    public void Check(object value);
}
```
