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

        public NullableReferenceFormProperty(ICollection<TModel> models, Expression<Func<TModel, T?>> propertySelector)
            : this(models, propertySelector, true)
        {
        }

        protected NullableReferenceFormProperty(ICollection<TModel> models, Expression<Func<TModel, T?>> propertySelector, bool allowNull)
            : base(models)
        {
            _allowNull = allowNull;
            _descriptor = new FormPropertyDescriptor<TModel, T?>(propertySelector, false);

            Initialize();
        }

        protected override (bool isSet, T? value) GetValue()
        {
            return _descriptor.GetValue(Models);
        }

        protected override bool TrySetValue(T? value)
        {
            if (_allowNull || value != null)
            {
                _descriptor.SetValue(Models, value);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
