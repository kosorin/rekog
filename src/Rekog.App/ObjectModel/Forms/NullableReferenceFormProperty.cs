using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public class NullableReferenceFormProperty<TModel, T> : FormProperty<TModel, T?>
        where TModel : ModelBase
        where T : class?
    {
        private readonly FormPropertyDescriptor<TModel, T?> _descriptor;
        private readonly bool _allowNull;

        public NullableReferenceFormProperty(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector)
            : this(form, propertySelector, true)
        {
        }

        protected NullableReferenceFormProperty(IForm<TModel> form, Expression<Func<TModel, T?>> propertySelector, bool allowNull)
            : base(form)
        {
            _allowNull = allowNull;
            _descriptor = new FormPropertyDescriptor<TModel, T?>(propertySelector, false);
        }

        protected override (bool isSet, T? value) GetValue(IReadOnlyCollection<TModel> models)
        {
            return _descriptor.GetValue(models);
        }

        protected override bool TrySetValue(IReadOnlyCollection<TModel> models, T? value)
        {
            if (_allowNull || value != null)
            {
                _descriptor.SetValue(models, value);
                return true;
            }
            return false;
        }
    }
}
