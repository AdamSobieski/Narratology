using System.Linq.Expressions;

namespace System
{
    public interface ILambdaGenerator : IInspectableDelegate
    {
        public new LambdaExpression Invoke(object?[]? args);
    }

    public interface ILambdaGenerator<TDelegate> : ILambdaGenerator
    {
        public new Expression<TDelegate> Invoke(object?[]? args);
    }

    public interface ILambdaGenerator<T, TDelegate> : ILambdaGenerator<TDelegate>, IInspectableFunc<T, Expression<TDelegate>> { }

    public interface ILambdaGenerator<T1, T2, TDelegate> : ILambdaGenerator<TDelegate>, IInspectableFunc<T1, T2, Expression<TDelegate>> { }

    public interface ILambdaGenerator<T1, T2, T3, TDelegate> : ILambdaGenerator<TDelegate>, IInspectableFunc<T1, T2, T3, Expression<TDelegate>> { }

    public interface ILambdaGenerator<T1, T2, T3, T4, TDelegate> : ILambdaGenerator<TDelegate>, IInspectableFunc<T1, T2, T3, T4, Expression<TDelegate>> { }
}
