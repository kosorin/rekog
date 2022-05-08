using System;
using System.Runtime.CompilerServices;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.Model
{
    public abstract class ModelBase : ObservableObject
    {
        protected override bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            var oldValue = field;

            if (!base.Set(ref field, value, propertyName))
            {
                return false;
            }

            OnUndoActionExecuted(new ChangePropertyUndoAction(propertyName ?? throw new ArgumentNullException(nameof(propertyName)), this, value, oldValue));

            return true;
        }

        protected virtual void OnUndoActionExecuted(IUndoAction undoAction)
        {
            UndoActionExecuted?.Invoke(this, undoAction);
        }

        public event UndoActionPublishedEventHandler? UndoActionExecuted;
    }
}
