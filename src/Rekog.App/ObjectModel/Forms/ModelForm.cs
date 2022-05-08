using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Undo;
using Rekog.App.ViewModel;
using Rekog.Common.Extensions;

namespace Rekog.App.ObjectModel.Forms
{
    public abstract class ModelForm : ViewModelBase
    {
    }

    public abstract class ModelForm<TModel> : ModelForm
        where TModel : ModelBase
    {
        private static readonly Dictionary<string, PropertyInfo> PropertyInfos = new Dictionary<string, PropertyInfo>();

        private readonly UndoContext _undoContext;
        private readonly Dictionary<string, ModelFormProperty> _properties = new Dictionary<string, ModelFormProperty>();
        private TModel[] _models = Array.Empty<TModel>();
        private bool _isUpdatingProperty;

        protected ModelForm(UndoContext undoContext)
        {
            _undoContext = undoContext;
        }

        public bool IsSet => _models.Length > 0;

        public void SetModel(TModel model)
        {
            SetModelsCore(new[] { model, });
        }

        public void SetModels(IEnumerable<TModel> models)
        {
            SetModelsCore(models.ToArray());
        }

        public void ClearModels()
        {
            SetModelsCore(Array.Empty<TModel>());
        }

        private void SetModelsCore(TModel[] models)
        {
            foreach (var model in _models)
            {
                model.PropertyChanged -= OnModelPropertyChanged;
            }

            _models = models;

            foreach (var model in _models)
            {
                model.PropertyChanged += OnModelPropertyChanged;
            }

            UpdateProperties();

            OnPropertyChanged(nameof(IsSet));
        }

        protected ModelFormProperty GetProperty(string name)
        {
            if (_properties.TryGetValue(name, out var property))
            {
                return property;
            }

            property = new ModelFormProperty(name, GetDefaultValue(name), OnPropertyValueChanged);

            UpdateProperty(property);

            _properties.Add(name, property);

            return property;
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (_properties.TryGetValue(args.PropertyName ?? throw new ArgumentException(null, nameof(args)), out var property))
            {
                UpdateProperty(property);
            }
        }

        private void OnPropertyValueChanged(ModelFormProperty property, object? value)
        {
            UpdateModels(property, value);
        }

        private void UpdateProperties()
        {
            foreach (var property in _properties.Values)
            {
                UpdateProperty(property);
            }
        }

        private void UpdateProperty(ModelFormProperty property)
        {
            _isUpdatingProperty = true;
            try
            {
                property.Value = GetValue(property.Name, _models);
            }
            finally
            {
                _isUpdatingProperty = false;
            }
        }

        private void UpdateModels(ModelFormProperty property, object? value)
        {
            if (_isUpdatingProperty)
            {
                return;
            }

            using (_undoContext.Batch())
            {
                SetValue(value ?? property.DefaultValue, property.Name, _models);

                if (value == null && property.DefaultValue != null)
                {
                    property.Value = property.DefaultValue;
                }
            }
        }

        private static object? GetValue(string propertyName, TModel[] models)
        {
            if (models.Length == 0)
            {
                return null;
            }

            var propertyInfo = GetPropertyInfo(propertyName);

            var value = propertyInfo.GetValue(models.First());

            if (models.Length == 1 || models.Skip(1).All(model => Equals(propertyInfo.GetValue(model), value)))
            {
                return value;
            }

            return null;
        }

        private static void SetValue(object? value, string propertyName, TModel[] models)
        {
            var propertyInfo = GetPropertyInfo(propertyName);

            foreach (var model in models)
            {
                propertyInfo.SetValue(model, value);
            }
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
            {
                return propertyInfo;
            }

            propertyInfo = typeof(TModel).GetProperty(propertyName) ?? throw new ArgumentException(null, nameof(propertyName));

            PropertyInfos.Add(propertyName, propertyInfo);

            return propertyInfo;
        }

        private static object? GetDefaultValue(string propertyName)
        {
            object? defaultValue = null;

            var propertyInfo = GetPropertyInfo(propertyName);
            if (!propertyInfo.IsNullable())
            {
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.IsValueType)
                {
                    defaultValue = Activator.CreateInstance(propertyType);
                }
                else if (propertyType == typeof(string))
                {
                    defaultValue = string.Empty;
                }
                else
                {
                    throw new NotSupportedException($"Type {propertyType.FullName} is not supported as a type for model form property.");
                }
            }

            return defaultValue;
        }
    }
}
