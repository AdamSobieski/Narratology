﻿using Microsoft.CodeAnalysis;

namespace Prototype.CodeAnalysis
{
    public sealed class Explained<T> { }

    public sealed class CausalAnalysis { }

    public static class Explainability
    {
        public static CausalAnalysis AnalyzeCausalFlow(this SemanticModel model, SyntaxNode statement)
        {
            throw new NotImplementedException();
        }

        public static Func<Explained<TResult>> ToExplainable<TResult>(this Func<TResult> method)
        {
            throw new NotImplementedException();
        }
        public static Func<T, Explained<TResult>> ToExplainable<T, TResult>(this Func<T, TResult> method)
        {
            throw new NotImplementedException();
        }
        public static Func<T1, T2, Explained<TResult>> ToExplainable<T1, T2, TResult>(this Func<T1, T2, TResult> method)
        {
            throw new NotImplementedException();
        }
        public static Func<T1, T2, T3, Explained<TResult>> ToExplainable<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> method)
        {
            throw new NotImplementedException();
        }
        public static Func<T1, T2, T3, T4, Explained<TResult>> ToExplainable<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> method)
        {
            throw new NotImplementedException();
        }
    }
}