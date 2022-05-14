using System.Reflection;
using Rekog.App.Reflection;

namespace Rekog.App.Undo.Actions
{
    public class ChangePropertyUndoAction : IUndoAction
    {
        public ChangePropertyUndoAction(object instance, string propertyName, object? newValue, object? oldValue)
        {
            PropertyInfo = ReflectionCache.GetPropertyInfo(instance, propertyName);
            Instance = instance;
            NewValue = newValue;
            OldValue = oldValue;
        }

        public PropertyInfo PropertyInfo { get; }

        public object Instance { get; }

        public object? NewValue { get; set; }

        public object? OldValue { get; set; }

        public void Undo()
        {
            PropertyInfo.SetValue(Instance, OldValue);
        }

        public void Redo()
        {
            PropertyInfo.SetValue(Instance, NewValue);
        }
    }
}
