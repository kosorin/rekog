using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public abstract class FormProperty<TModel> : ObservableObject
        where TModel : ModelBase
    {
        protected FormProperty(IForm<TModel> form)
        {
            Form = form;
        }

        protected IForm<TModel> Form { get; }

        public abstract string Name { get; }

        public abstract void Update();
    }

    public abstract class FormProperty<TModel, T> : FormProperty<TModel>
        where TModel : ModelBase
    {
        private bool _isUpdating;
        private bool _isSet;
        private T? _value;

        protected FormProperty(IForm<TModel> form)
            : base(form)
        {
        }

        public bool IsSet
        {
            get => _isSet;
            private set => Set(ref _isSet, value);
        }

        public T? Value
        {
            get => _isSet ? _value : default;
            set
            {
                if (Set(ref _value, value))
                {
                    if (!_isUpdating)
                    {
                        IsSet = TrySetValue(Form.Models, value);
                    }
                }
            }
        }

        public sealed override void Update()
        {
            try
            {
                _isUpdating = true;

                var (isSet, value) = GetValue(Form.Models);
                IsSet = isSet;
                Value = isSet ? value : default;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected abstract (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models);

        protected abstract bool TrySetValue(IReadOnlyCollection<TModel> models, T? value);
    }

    public static class FormProperty
    {
        public static FormProperty<TModel, T?> Value<TModel, T>(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            where TModel : ModelBase
            where T : struct
        {
            return new ValueFormProperty<TModel, T>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> NullableValue<TModel, T>(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector)
            where TModel : ModelBase
        {
            return new NullableValueFormProperty<TModel, T?>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> Reference<TModel, T>(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            where TModel : ModelBase
            where T : class
        {
            return new ReferenceFormProperty<TModel, T>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> NullableReference<TModel, T>(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector)
            where TModel : ModelBase
            where T : class?
        {
            return new NullableReferenceFormProperty<TModel, T?>(form, propertySelector);
        }
    }
}
