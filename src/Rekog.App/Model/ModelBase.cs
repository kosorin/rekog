using Rekog.App.ObjectModel;
using Rekog.App.Undo;
using Rekog.App.Undo.Actions;

namespace Rekog.App.Model
{
    public abstract class ModelBase : ObservableObject
    {
        protected override void OnPropertyChanged(string propertyName, object? newValue, object? oldValue)
        {
            base.OnPropertyChanged(propertyName, newValue, oldValue);

            UndoActionExecuted?.Invoke(this, new ChangePropertyUndoAction(this, propertyName, newValue, oldValue));
        }

        public event UndoActionPublishedEventHandler? UndoActionExecuted;
    }
}
