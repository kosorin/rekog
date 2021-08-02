using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel.Forms
{
    public abstract class FormProperty : ObservableObject
    {
        public static IFormProperty<T?> Value<TModel, T>(ICollection<TModel> models, Expression<Func<TModel, T>> propertySelector)
            where TModel : ModelBase
            where T : struct
        {
            return new ValueFormProperty<TModel, T>(models, propertySelector);
        }

        public static IFormProperty<T?> NullableValue<TModel, T>(ICollection<TModel> models, Expression<Func<TModel, T?>> propertySelector)
            where TModel : ModelBase
        {
            return new NullableValueFormProperty<TModel, T?>(models, propertySelector);
        }

        public static IFormProperty<T?> Reference<TModel, T>(ICollection<TModel> models, Expression<Func<TModel, T>> propertySelector)
            where TModel : ModelBase
            where T : class
        {
            return new ReferenceFormProperty<TModel, T>(models, propertySelector);
        }

        public static IFormProperty<T?> NullableReference<TModel, T>(ICollection<TModel> models, Expression<Func<TModel, T?>> propertySelector)
            where TModel : ModelBase
            where T : class?
        {
            return new NullableReferenceFormProperty<TModel, T?>(models, propertySelector);
        }
    }

    public abstract class FormProperty<TModel, T> : FormProperty, IFormProperty<T?>
    {
        private bool _isSet;
        private T? _value;

        protected FormProperty(ICollection<TModel> models)
        {
            Models = models;
        }

        public ICollection<TModel> Models { get; }

        public bool IsSet
        {
            get => _isSet;
            private set => Set(ref _isSet, value);
        }

        public T? Value
        {
            get => _value;
            set
            {
                if (Set(ref _value, value))
                {
                    IsSet = TrySetValue(value);
                }
            }
        }

        protected void Initialize()
        {
            // Set values to fields instead of properties to prevent side effects
            (_isSet, _value) = GetValue();
        }

        protected abstract (bool isSet, T? value) GetValue();

        protected abstract bool TrySetValue(T? value);
    }
}
