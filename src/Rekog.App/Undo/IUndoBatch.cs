namespace Rekog.App.Undo
{
    public interface IUndoBatch
    {
        void Undo();

        void Redo();
    }
}
