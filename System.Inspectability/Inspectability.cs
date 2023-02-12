using System.Linq.Expressions;

namespace System
{
    public interface IInspectableMethod
    {
        public LambdaExpression Expression { get; }

        public object? Invoke(object?[]? args);
    }

    public interface IInspectableFunc<T, TResult> : IInspectableMethod
    {
        public new Expression<Func<T, TResult>> Expression { get; }

        public TResult Invoke(T arg);
    }

    public interface IInspectableFunc<T1, T2, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableFunc<T1, T2, T3, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, T5, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : IInspectableMethod
    {
        public new Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> Expression { get; }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }

    public interface IInspectableAction<T> : IInspectableMethod
    {
        public new Expression<Action<T>> Expression { get; }

        public void Invoke(T arg);
    }

    public interface IInspectableAction<T1, T2> : IInspectableMethod
    {
        public new Expression<Action<T1, T2>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableAction<T1, T2, T3> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableAction<T1, T2, T3, T4> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4, T5>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4, T5, T6>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6, T7> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4, T5, T6, T7>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6, T7, T8> : IInspectableMethod
    {
        public new Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Expression { get; }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }
}