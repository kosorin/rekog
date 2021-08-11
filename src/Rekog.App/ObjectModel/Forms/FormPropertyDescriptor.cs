using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rekog.App.ObjectModel.Forms
{
    public class FormPropertyDescriptor<TModel, T>
    {
        private readonly Func<TModel, T?> _getter;

        private readonly Action<TModel, T?> _setter;

        public FormPropertyDescriptor(Expression<Func<TModel, T>> propertySelector, [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")] bool isValueType)
        {
            if (typeof(T).IsValueType != isValueType)
            {
                throw new InvalidOperationException();
            }

            var property = GetProperty(propertySelector);
            _getter = model => (T?)property.GetValue(model);
            _setter = (model, value) => property.SetValue(model, value);

            Name = $"{typeof(TModel).Name}:{property.Name}";
        }

        // For debug purpose
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Name { get; }

        public (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models)
        {
            if (models.Count == 0)
            {
                return (false, default);
            }

            var value = _getter.Invoke(models.First());

            if (models.Count == 1 || models.Skip(1).All(x => EqualityComparer<T>.Default.Equals(_getter.Invoke(x), value)))
            {
                return (true, value);
            }

            return (false, default);
        }

        public void SetValue(IReadOnlyCollection<TModel> models, T? value)
        {
            foreach (var model in models)
            {
                _setter.Invoke(model, value);
            }
        }

        private static PropertyInfo GetProperty(Expression<Func<TModel, T>> propertySelector)
        {
            if (propertySelector.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException(null, nameof(propertySelector));
            }

            var modelType = typeof(TModel);
            if (modelType.GetProperty(memberExpression.Member.Name) is not { } property)
            {
                throw new ArgumentException(null, nameof(propertySelector));
            }

            if (typeof(T) != property.PropertyType)
            {
                throw new ArgumentException(null, nameof(propertySelector));
            }

            return property;
        }
    }
}
