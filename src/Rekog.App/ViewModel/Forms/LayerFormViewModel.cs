using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class LayerFormViewModel : FormViewModel<LayerModel>
    {
        public LayerFormViewModel()
        {
            LegendForm.Clear();

            Properties = new FormProperty<LayerModel>[]
            {
                Name = FormProperty<LayerModel>.Reference(this, x => x.Name),
            };
        }

        public LegendFormViewModel LegendForm { get; } = new LegendFormViewModel();

        public override IReadOnlyCollection<FormProperty<LayerModel>> Properties { get; }

        public FormProperty<LayerModel, string?> Name { get; }
    }
}
