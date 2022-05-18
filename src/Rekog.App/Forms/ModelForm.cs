using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Rekog.App.Extensions;
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

        private bool _isUpdatingProperty;

        protected ModelForm(UndoContext undoContext)
        {
            _undoContext = undoContext;
        }

        public bool IsSet => Models.Length > 0;

        public TModel[] Models { get; private set; } = Array.Empty<TModel>();

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
            foreach (var model in Models)
            {
                model.PropertyChanged -= OnModelPropertyChanged;
            }

            Models = models;

            foreach (var model in Models)
            {
                model.PropertyChanged += OnModelPropertyChanged;
            }

            OnModelsChanged();
        }

        protected virtual void OnModelsChanged()
        {
            UpdateProperties();

            OnPropertyChanged(nameof(IsSet));
            OnPropertyChanged(nameof(Models));
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

        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (_properties.TryGetValue(args.PropertyName ?? throw new ArgumentException(null, nameof(args)), out var property))
            {
                UpdateProperty(property);
            }
        }

        protected virtual void OnPropertyValueChanged(ModelFormProperty property, object? value)
        {
            UpdateModels(property, value);
        }

        private ModelFormProperty AddProperty(string name, object? defaultValue)
        {
            var property = new ModelFormProperty(name, defaultValue, OnPropertyValueChanged);

            UpdateProperty(property);

            _properties.Add(name, property);

            return property;
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
                var value = Models.GetSameOrDefaultValue(propertyInfo.GetValue);
                property.Value = value;
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

                value ??= property.DefaultValue;
                foreach (var model in Models)
                {
                    propertyInfo.SetValue(model, value);
                }

                // If default value was used, set it back to property
                if (usedDefaultValue)
                {
                    property.Value = property.DefaultValue;
                }
            }
        }
    }
}
