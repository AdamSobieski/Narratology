using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System
{
    public interface IInspectableDelegate
    {
        public MethodDeclarationSyntax Syntax { get; }

        public object? Invoke(object?[]? args);
    }

    public interface IInspectableFunc<TResult> : IInspectableDelegate
    {
        public TResult Invoke();
    }

    public interface IInspectableFunc<T, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T arg);
    }

    public interface IInspectableFunc<T1, T2, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableFunc<T1, T2, T3, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : IInspectableDelegate
    {
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }

    public interface IInspectableAction : IInspectableDelegate
    {
        public void Invoke();
    }

    public interface IInspectableAction<T> : IInspectableDelegate
    {
        public void Invoke(T arg);
    }

    public interface IInspectableAction<T1, T2> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2);
    }

    public interface IInspectableAction<T1, T2, T3> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IInspectableAction<T1, T2, T3, T4> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6, T7> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }

    public interface IInspectableAction<T1, T2, T3, T4, T5, T6, T7, T8> : IInspectableDelegate
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }
}