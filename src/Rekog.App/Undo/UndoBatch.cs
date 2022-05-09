using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.Undo
{
    public class UndoBatch
    {
        private readonly IUndoAction[] _actions;

        public UndoBatch(IEnumerable<IUndoAction> actions)
        {
            _actions = actions.ToArray();
        }

        public void Undo()
        {
            foreach (var action in _actions.Reverse())
            {
                action.Undo();
            }
        }

        public void Redo()
        {
            foreach (var action in _actions)
            {
                action.Redo();
            }
        }
    }
}
