using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.Loader;

namespace System.Inspectability
{
    public static class RuntimeCompiler
    {
        public static Delegate Compile(this MethodDeclarationSyntax method)
        {
            return Compile(method, AssemblyLoadContext.Default);
        }
        public static Delegate Compile(this MethodDeclarationSyntax method, AssemblyLoadContext context)
        {
            throw new NotImplementedException();
        }
    }
}
