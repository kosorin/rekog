using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.DesignTime
{
    public abstract class ColorFormProperty : FormProperty<ColorFormProperty.Model, string>
    {
        protected ColorFormProperty(IForm<Model> form) : base(form)
        {
        }

        protected override (bool isSet, string? value) GetValue(IReadOnlyCollection<Model> models)
        {
            throw new System.NotSupportedException();
        }

        protected override bool TrySetValue(IReadOnlyCollection<Model> models, string? value)
        {
            throw new System.NotSupportedException();
        }

        public abstract class Model : ModelBase
        {
        }
    }
}
