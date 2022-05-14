namespace Rekog.App.Undo
{
    public interface IUndoBatchBuilder
    {
        void Initialize();

        void PushAction(IUndoAction action);

        bool TryCoalesce(IUndoBatch lastBatch);

        IUndoBatch? Build();
    }
}
