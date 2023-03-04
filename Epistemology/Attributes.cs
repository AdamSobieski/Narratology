using AI.Epistemology;
using Microsoft.CodeAnalysis;

namespace System
{
    public abstract class Describer : Attribute
    {
        protected Describer() { }

        public abstract IStatementCollection Invoke(object value);
    }

    public abstract class Validator : Attribute
    {
        protected Validator() { }

        public abstract bool Invoke(object value);
    }

    public abstract class Transformer : Attribute
    {
        protected Transformer() { }

        public abstract object Invoke(object value);
    }

    public abstract class ReversibleTransformer : Transformer
    {
        protected ReversibleTransformer() { }

        public abstract object Undo(object value);
    }

    public abstract class CodeAnalysisValidator : Validator
    {
        protected CodeAnalysisValidator() { }

        public sealed override bool Invoke(object value)
        {
            foreach(var diagnostic in Analyze(value))
            {
                if(diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.IsWarningAsError)
                {
                    return false;
                }
            }
            return true;
        }

        public abstract IEnumerable<Diagnostic> Analyze(object value);
    }
}