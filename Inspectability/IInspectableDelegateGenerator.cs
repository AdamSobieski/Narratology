namespace System
{
    public interface IInspectableDelegateGenerator : IInspectableDelegate
    {
        public IInspectableDelegate Invoke(object?[]? args);
    }
}
