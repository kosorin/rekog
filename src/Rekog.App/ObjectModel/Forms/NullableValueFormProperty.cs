using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public class NullableValueFormProperty<TModel, T> : FormProperty<TModel, T?>
        where TModel : ModelBase
    {
        private readonly FormPropertyDescriptor<TModel, T> _descriptor;

        public NullableValueFormProperty(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            : base(form)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                throw new Exception($"Generic parameter {nameof(T)} of type {nameof(NullableValueFormProperty<TModel, T>)} expects {nameof(Nullable)} type.");
            }

            _descriptor = new FormPropertyDescriptor<TModel, T>(propertySelector, true);
        }

        public override string Name => _descriptor.Name;

        protected override (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models)
        {
            return _descriptor.GetValue(models);
        }

        protected override bool TrySetValue(IReadOnlyCollection<TModel> models, T? value)
        {
            _descriptor.SetValue(models, value);
            return true;
        }
    }
}
