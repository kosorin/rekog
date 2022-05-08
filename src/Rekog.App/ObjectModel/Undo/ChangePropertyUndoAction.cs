using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rekog.App.ObjectModel.Undo
{
    public class ChangePropertyUndoAction : IUndoAction
    {
        private static readonly Dictionary<(Type type, string propertyName), PropertyInfo> PropertyInfos = new Dictionary<(Type type, string propertyName), PropertyInfo>();

        private readonly PropertyInfo _propertyInfo;
        private readonly object _instance;
        private readonly object? _newValue;
        private readonly object? _oldValue;

        public ChangePropertyUndoAction(string propertyName, object instance, object? newValue, object? oldValue)
        {
            _propertyInfo = GetPropertyInfo(propertyName, instance);

            _instance = instance;
            _newValue = newValue;
            _oldValue = oldValue;
        }

        private ChangePropertyUndoAction(PropertyInfo propertyInfo, object instance, object? newValue, object? oldValue)
        {
            _propertyInfo = propertyInfo;

            _instance = instance;
            _newValue = newValue;
            _oldValue = oldValue;
        }

        public void Undo()
        {
            _propertyInfo.SetValue(_instance, _oldValue);
        }

        public void Redo()
        {
            _propertyInfo.SetValue(_instance, _newValue);
        }

        public IUndoAction? TryConsolidate(IUndoAction previousAction)
        {
            if (previousAction is not ChangePropertyUndoAction previous)
            {
                return null;
            }

            if (previous._instance != _instance || previous._propertyInfo.Name != _propertyInfo.Name)
            {
                return null;
            }

            if (Equals(_newValue, previous._oldValue))
            {
                return null;
            }

            return new ChangePropertyUndoAction(_propertyInfo, _instance, _newValue, previous._oldValue);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName, object instance)
        {
            var type = instance.GetType();
            var key = (type, propertyName);

            if (PropertyInfos.TryGetValue(key, out var propertyInfo))
            {
                return propertyInfo;
            }

            propertyInfo = type.GetProperty(propertyName);

            PropertyInfos.Add(key, propertyInfo ?? throw new ArgumentException(null, nameof(propertyName)));

            return propertyInfo;
        }
    }
}