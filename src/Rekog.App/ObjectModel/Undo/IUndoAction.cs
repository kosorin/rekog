namespace Rekog.App.ObjectModel.Undo
{
    public interface IUndoAction
    {
        void Undo();

        void Redo();
    }
}