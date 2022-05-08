using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class LayerModelForm : ModelForm<LayerModel>
    {
        public LayerModelForm(UndoContext undoContext) : base(undoContext)
        {
            LegendForm = new LegendModelForm(undoContext);
        }

        public LegendModelForm LegendForm { get; }

        public ModelFormProperty Name => GetProperty(nameof(LayerModel.Name));
    }
}
