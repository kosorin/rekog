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

        public NullableValueFormProperty(ICollection<TModel> models, Expression<Func<TModel, T>> propertySelector)
            : base(models)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                throw new Exception($"Generic parameter {nameof(T)} of type {nameof(NullableValueFormProperty<TModel, T>)} expects {nameof(Nullable)} type.");
            }

            _descriptor = new FormPropertyDescriptor<TModel, T>(propertySelector, true);

            Initialize();
        }

        protected override (bool isSet, T? value) GetValue()
        {
            return _descriptor.GetValue(Models);
        }

        protected override bool TrySetValue(T? value)
        {
            _descriptor.SetValue(Models, value);
            return true;
        }
    }
}
