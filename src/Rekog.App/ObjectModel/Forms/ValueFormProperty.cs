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

        public ValueFormProperty(ICollection<TModel> models, Expression<Func<TModel, T>> propertySelector)
            : base(models)
        {
            _descriptor = new FormPropertyDescriptor<TModel, T>(propertySelector, true);

            Initialize();
        }

        protected override (bool isSet, T? value) GetValue()
        {
            return _descriptor.GetValue(Models);
        }

        protected override bool TrySetValue(T? value)
        {
            if (value.HasValue)
            {
                _descriptor.SetValue(Models, value.Value);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
