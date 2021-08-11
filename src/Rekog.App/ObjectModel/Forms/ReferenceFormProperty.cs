using System;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public class ReferenceFormProperty<TModel, T> : NullableReferenceFormProperty<TModel, T>
        where TModel : ModelBase
        where T : class
    {
        public ReferenceFormProperty(IForm<TModel> form, Expression<Func<TModel, T>> propertySelector)
            : base(form, propertySelector!, false)
        {
        }
    }
}
