using System;

namespace Rekog.App.Undo.Actions
{
    public class DelegateUndoAction : IUndoAction
    {
        private readonly Action? _undo;
        private readonly Action? _redo;

        public DelegateUndoAction(Action? undo, Action? redo)
        {
            _undo = undo;
            _redo = redo;
        }

        public void Undo()
        {
            _undo?.Invoke();
        }

        public void Redo()
        {
            _redo?.Invoke();
        }
    }
}
