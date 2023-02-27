using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExplainedByMethodAttribute : Attribute
    {
        public ExplainedByMethodAttribute(Type type, string methodName)
        {
            Type = type;
            MethodName = methodName;
        }

        public Type Type { get; }
        public string MethodName { get; }
    }

    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    //public sealed class ExplainsMethodAttribute : Attribute
    //{
    //    public ExplainsMethodAttribute(Type type, string methodName)
    //    {
    //        Type = type;
    //        MethodName = methodName;
    //    }

    //    public Type Type { get; }
    //    public string MethodName { get; }
    //}
}

namespace Prototype.CodeAnalysis
{
    public sealed class Explained<T>
    {
        public Explained(T value)
        {
            Value = value;
        }

        public T Value { get; }

        // ...
    }

    public sealed class CausalFlowAnalysis
    {
        // see also: https://github.com/dotnet/roslyn-analyzers/tree/main/src/Utilities/FlowAnalysis/FlowAnalysis
    }

    public static class Explainability
    {
        public static CausalFlowAnalysis AnalyzeCausalFlow(this SemanticModel model, SyntaxNode statement)
        {
            throw new NotImplementedException();
        }

        public static Func<Explained<TResult>> ToExplainable<TResult>(this Func<TResult> method)
        {
            // Algorithm rough draft:

            // Step 1: Does the input method have a metadata attribute indicating that it was compiled with an accompanying explainable method?

            object[] attributes = method.Method.GetCustomAttributes(typeof(ExplainedByMethodAttribute), false);
            if (attributes.Length > 0)
            {
                if (attributes.Length > 1)
                {
                    throw new InvalidProgramException();
                }

                var attribute = (ExplainedByMethodAttribute)attributes[0];

                // ...
            }
            else
            {
                // Step 2: If not, is there a cached result for this input method?

                // Step 3: If not, decompile the input method

                // Step 4: Transform the ICSharpCode.Decompiler syntax tree into a Microsoft.CodeAnalysis.CSharp syntax tree using a visitor, e.g., extending DepthFirstAstVisitor<object>

                // Step 5: Obtain a Microsoft.CodeAnalysis.CSharp semantic model for the resultant syntax tree
                
                // Step 6: Perform a configurable causal flow analysis on the Microsoft.CodeAnalysis.CSharp semantic model

                // Step 7: Obtain data, the explanations of each return or yield statement, with which to construct each Explained<T> value for each resultant return or yield statement

                // Step 8: Generate the output method (add CompilerGenerated attribute to it, add ExplainsMethod attribute to it (?), etc.)

                // Step 9: Compile the output method

                // Step 10: Return a delegate for the compiled output method
            }
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