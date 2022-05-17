namespace Rekog.App.Undo
{
    public interface IUndoBatchBuilder
    {
        void Initialize();

        void PushAction(IUndoAction action);

        UndoCoalesceResult Coalesce(IUndoBatch lastBatch);

        IUndoBatch? Build();
    }
}
