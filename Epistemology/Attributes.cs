using AI.Epistemology;
using Microsoft.CodeAnalysis;

namespace System
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class MethodBodyAttributeAttribute : Attribute
    {
        public MethodBodyAttributeAttribute(string fileName, int startLine, int startColumn, int endLine, int endColumn, int ilOffset, Type attributeType, params object?[]? ctorArguments)
        {

        }
        public MethodBodyAttributeAttribute(int ilOffset, Type attributeType, params object?[]? ctorArguments)
        {

        }
    }

    public abstract class Describer : Attribute
    {
        protected Describer() { }

        public abstract IStatementCollection Invoke(object value, object context);
    }

    public abstract class Validator : Attribute
    {
        protected Validator() { }

        public abstract bool Invoke(object value, object context);
    }

    public abstract class Transformer : Attribute
    {
        protected Transformer() { }

        public abstract object Invoke(object value, object context);
    }

    public abstract class ReversibleTransformer : Transformer
    {
        protected ReversibleTransformer() { }

        public abstract object Undo(object value, object context);
    }

    public abstract class CodeAnalysisValidator : Validator
    {
        protected CodeAnalysisValidator() { }

        public sealed override bool Invoke(object value, object context)
        {
            foreach (var diagnostic in Analyze(value, context))
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.IsWarningAsError)
                {
                    return false;
                }
            }
            return true;
        }

        public abstract IEnumerable<Diagnostic> Analyze(object value, object context);
    }
}