namespace Rekog.App.Undo
{
    public interface IUndoAction
    {
        void Undo();

        void Redo();
    }
}
