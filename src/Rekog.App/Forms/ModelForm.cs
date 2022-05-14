using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Rekog.App.Model;
using Rekog.App.Reflection;
using Rekog.App.Undo;
using Rekog.App.Undo.Batches;
using Rekog.Common.Extensions;

namespace Rekog.App.Forms
{
    public abstract class ModelForm<TModel> : Form
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

            return AddProperty(name, ReflectionCache.GetPropertyInfo<TModel>(name).GetDefaultValue());
        }

        protected ModelFormProperty GetProperty(string name, object? defaultValue)
        {
            if (_properties.TryGetValue(name, out var property))
            {
                return property;
            }

            return AddProperty(name, defaultValue);
        }

        private ModelFormProperty AddProperty(string name, object? defaultValue)
        {
            var property = new ModelFormProperty(name, defaultValue, OnPropertyValueChanged);

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
                var propertyInfo = ReflectionCache.GetPropertyInfo<TModel>(property.Name);

                property.Value = GetModelsValue(propertyInfo);
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

            var propertyInfo = ReflectionCache.GetPropertyInfo<TModel>(property.Name);

            using (_undoContext.Batch(new ChangePropertyUndoBatchBuilder(new PropertyInfoChangePropertyGroupKey(propertyInfo))))
            {
                var usedDefaultValue = value == null;

                SetModelsValue(propertyInfo, value ?? property.DefaultValue);

                // If default value was used, set it back to property
                if (usedDefaultValue)
                {
                    property.Value = property.DefaultValue;
                }
            }
        }

        private object? GetModelsValue(PropertyInfo propertyInfo)
        {
            if (_models.Length == 0)
            {
                return null;
            }

            var value = propertyInfo.GetValue(_models.First());

            if (_models.Length == 1 || _models.Skip(1).All(model => Equals(propertyInfo.GetValue(model), value)))
            {
                return value;
            }

            return null;
        }

        private void SetModelsValue(PropertyInfo propertyInfo, object? value)
        {
            foreach (var model in _models)
            {
                propertyInfo.SetValue(model, value);
            }
        }
    }
}
