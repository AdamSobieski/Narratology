using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace System.Inspectability
{
    public static class RuntimeCompiler
    {
        public static Delegate Compile(this SyntaxNode method)
        {
            return Compile(method, AssemblyLoadContext.Default);
        }
        public static Delegate Compile(this SyntaxNode method, AssemblyLoadContext context)
        {
            throw new NotImplementedException();
        }
    }
}
