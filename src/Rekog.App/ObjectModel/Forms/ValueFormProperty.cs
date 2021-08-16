using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public class ValueFormProperty<TModel, T> : FormProperty<TModel, T?>
        where TModel : ModelBase
        where T : struct
    {
        private readonly FormPropertyDescriptor<TModel, T> _descriptor;

        public ValueFormProperty(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            : base(form)
        {
            _descriptor = new FormPropertyDescriptor<TModel, T>(propertySelector, true);
        }

        protected override (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models)
        {
            return _descriptor.GetValue(models);
        }

        protected override bool TrySetValue(IReadOnlyCollection<TModel> models, T? value)
        {
            if (value.HasValue)
            {
                _descriptor.SetValue(models, value.Value);
                return true;
            }
            return false;
        }
    }
}
