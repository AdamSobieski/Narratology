# Constraints

## Introduction

Here is an example of a usage of the constraint system described below, a fluent interface for building constraints for objects:

> [!NOTE]
> 
> ```cs
> var constraints = Constraint.Builder<DeterministicAcceptor>()
>     .Invariant(x => x.Start.Count() == 1)
>     .Declare(x => x.GetNavigator(), b => b.Invariant(x => x.Current.Count() == 1))
>     .Declare(x => x.GetNavigator(), b => b.Invariant(x => x.Edges.Count() == 1))
>     .Build();
> ```

## Interfaces

Here are some interfaces for a constraints system utilizing both `System.Linq.Expression` lambda expressions and `Proposition<bool>` Boolean propositions from the [_Knowledge_](Knowledge.md) system.

```cs
public interface IHasConstraints
{
    IConstraintsCollection Constraints { get; }
}

public interface IConstraintsCollection
{
    bool Check(object value);

    IConstraintsCollection PromoteMatchingDeclarations(LambdaExpression map);
}

public interface IConstraintsCollection<T> : IConstraintsCollection
{
    bool Check(T value);

    IConstraintsCollection<U> PromoteMatchingDeclarations<U>(Expression<Func<T, U>> map);
}

public interface IConstraintsBuilder<T>
{
    IConstraintsBuilder<T> Declare<U>(Expression<Func<T, U>> map, Expression<Action<IConstraintsBuilder<U>>> action);

    IConstraintsBuilder<T> Assert(Expression<Func<T, bool>> assertion, string? message = null);
    IConstraintsBuilder<T> Assert(Expression<Func<T, Proposition<bool>>> assertion, string? message = null);

    IConstraintsBuilder<T> Invariant(Expression<Func<T, bool>> predicate, string? message = null);
    IConstraintsBuilder<T> Invariant(Expression<Func<T, Proposition<bool>>> predicate, string? message = null);

    IConstraintsBuilder<T> When(Expression<Func<T, bool>> condition, Expression<Action<IConstraintsBuilder<T>>> action, string? message = null);
    IConstraintsBuilder<T> When(Expression<Func<T, Proposition<bool>>> condition, Expression<Action<IConstraintsBuilder<T>>> action, string? message = null);

    IConstraintsCollection<T> Build();
    IConstraintsCollection<T> Build(IReadOnlyKnowledge<bool> kb);
    IConstraintsCollection<T> Build(Func<T, IReadOnlyKnowledge<bool>> selector);
}
```
