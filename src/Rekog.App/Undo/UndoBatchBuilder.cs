using System.Collections.Generic;

namespace Rekog.App.Undo
{
    public class UndoBatchBuilder : IUndoBatchBuilder
    {
        private readonly List<IUndoAction> _actions = new List<IUndoAction>();

        public void Initialize()
        {
            _actions.Clear();
        }

        public void PushAction(IUndoAction action)
        {
            _actions.Add(action);
        }

        public UndoCoalesceResult Coalesce(IUndoBatch lastBatch)
        {
            return UndoCoalesceResult.None;
        }

        public IUndoBatch? Build()
        {
            return _actions.Count > 0
                ? new UndoBatch(_actions)
                : null;
        }
    }
}
