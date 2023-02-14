using System.Linq.Expressions;

namespace System
{
    public interface ILambdaGenerator : IInspectableMethod
    {
        public new LambdaExpression Invoke(object?[]? args);
    }

    public interface ILambdaGenerator<T> : ILambdaGenerator, IInspectableFunc<T, LambdaExpression> { }
}
