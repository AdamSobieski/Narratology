namespace System
{
    public interface IMethodGenerator : IInspectableDelegate
    {
        public new IInspectableDelegate Invoke(object?[]? args);
    }
}
