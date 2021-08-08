using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public class ReferenceFormProperty<TModel, T> : NullableReferenceFormProperty<TModel, T>
        where TModel : ModelBase
        where T : class
    {
        public ReferenceFormProperty(ICollection<TModel> models, Expression<Func<TModel, T>> propertySelector)
            : base(models, propertySelector!, false)
        {
        }
    }
}
