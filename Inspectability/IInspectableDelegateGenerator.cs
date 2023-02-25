namespace System
{
    public interface IInspectableDelegateGenerator : IInspectableDelegate
    {
        public new IInspectableDelegate Invoke(object?[]? args);
    }
}
