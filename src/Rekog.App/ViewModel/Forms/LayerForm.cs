using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class LayerForm : ModelForm<LayerModel>
    {
        public LayerForm(UndoContext undoContext) : base(undoContext)
        {
            LegendForm = new LegendForm(undoContext);
        }

        public LegendForm LegendForm { get; }

        public ModelFormProperty Name => GetProperty(nameof(LayerModel.Name));
    }
}
