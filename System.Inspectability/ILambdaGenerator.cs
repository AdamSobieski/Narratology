using System.Linq.Expressions;

namespace System
{
    public interface ILambdaGenerator : IInspectableDelegate
    {
        public new LambdaExpression Invoke(object?[]? args);
    }

    public interface ILambdaGenerator<T> : ILambdaGenerator, IInspectableFunc<T, LambdaExpression> { }

    public interface ILambdaGenerator<T1, T2> : ILambdaGenerator, IInspectableFunc<T1, T2, LambdaExpression> { }

    public interface ILambdaGenerator<T1, T2, T3> : ILambdaGenerator, IInspectableFunc<T1, T2, T3, LambdaExpression> { }

    public interface ILambdaGenerator<T1, T2, T3, T4> : ILambdaGenerator, IInspectableFunc<T1, T2, T3, T4, LambdaExpression> { }
}
