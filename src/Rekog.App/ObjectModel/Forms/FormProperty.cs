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

        public IForm<TModel> Form { get; }

        public abstract void Update();

        public static FormProperty<TModel, T?> Value<T>(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            where T : struct
        {
            return new ValueFormProperty<TModel, T>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> NullableValue<T>(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector)
        {
            return new NullableValueFormProperty<TModel, T?>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> Reference<T>(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            where T : class
        {
            return new ReferenceFormProperty<TModel, T>(form, propertySelector);
        }

        public static FormProperty<TModel, T?> NullableReference<T>(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector)
            where T : class?
        {
            return new NullableReferenceFormProperty<TModel, T?>(form, propertySelector);
        }
    }

    public abstract class FormProperty<TModel, T> : FormProperty<TModel>
        where TModel : ModelBase
    {
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
                    IsSet = TrySetValue(Form.Models, value);
                }
            }
        }

        public sealed override void Update()
        {
            var (isSet, value) = GetValue(Form.Models);
            IsSet = isSet;
            Value = isSet ? value : default;
        }

        // Warning: This method returns not null value for "Value" property even if isSet is false
        protected abstract (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models);

        protected abstract bool TrySetValue(IReadOnlyCollection<TModel> models, T? value);
    }
}
