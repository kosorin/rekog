using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rekog.App.Undo.Actions;

namespace Rekog.App.Undo.Batches
{
    public class ChangePropertyUndoBatchBuilder : IUndoBatchBuilder
    {
        private readonly IChangePropertyGroupKey _groupKey;

        private readonly List<IUndoAction> _actions = new List<IUndoAction>();
        private bool _isPure;

        public ChangePropertyUndoBatchBuilder(IChangePropertyGroupKey groupKey)
        {
            _groupKey = groupKey;
        }

        public void Initialize()
        {
            _actions.Clear();
            _isPure = true;
        }

        public void PushAction(IUndoAction action)
        {
            if (action is not ChangePropertyUndoAction { PropertyInfo: var propertyInfo, } || !_groupKey.AllowPropertyInfo(propertyInfo))
            {
                _isPure = false;
            }

            _actions.Add(action);
        }

        public bool TryCoalesce(IUndoBatch lastBatch)
        {
            if (!_isPure)
            {
                return false;
            }

            if (lastBatch is not Batch batch || !_groupKey.CanCoalesce(batch.GroupKey))
            {
                return false;
            }

            batch.Update(_actions.Cast<ChangePropertyUndoAction>());

            return true;
        }

        public IUndoBatch? Build()
        {
            return _actions.Count > 0
                ? _isPure
                    ? new Batch(_actions.Cast<ChangePropertyUndoAction>(), _groupKey)
                    : new UndoBatch(_actions)
                : null;
        }

        private class Batch : IUndoBatch
        {
            private readonly Dictionary<(object instance, PropertyInfo propertyInfo), ChangePropertyUndoAction> _actions = new Dictionary<(object instance, PropertyInfo propertyInfo), ChangePropertyUndoAction>();

            public Batch(IEnumerable<ChangePropertyUndoAction> actions, IChangePropertyGroupKey groupKey)
            {
                GroupKey = groupKey;

                Update(actions);
            }

            public IChangePropertyGroupKey GroupKey { get; }

            public void Update(IEnumerable<ChangePropertyUndoAction> actions)
            {
                foreach (var newAction in actions)
                {
                    var actionKey = (newAction.Instance, newAction.PropertyInfo);
                    if (_actions.TryGetValue(actionKey, out var currentAction))
                    {
                        currentAction.NewValue = newAction.NewValue;
                    }
                    else
                    {
                        _actions.Add(actionKey, newAction);
                    }
                }
            }

            public void Undo()
            {
                foreach (var action in _actions.Values)
                {
                    action.Undo();
                }
            }

            public void Redo()
            {
                foreach (var action in _actions.Values)
                {
                    action.Redo();
                }
            }
        }
    }
}
