using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class LegendModelForm : ModelForm<LegendModel>
    {
        public LegendModelForm(UndoContext undoContext) : base(undoContext)
        {
        }
        
        public ModelFormProperty Value => GetProperty(nameof(LegendModel.Value));

        public ModelFormProperty Alignment => GetProperty(nameof(LegendModel.Alignment));

        public ModelFormProperty Left => GetProperty(nameof(LegendModel.Left));

        public ModelFormProperty Top => GetProperty(nameof(LegendModel.Top));

        public ModelFormProperty Right => GetProperty(nameof(LegendModel.Right));

        public ModelFormProperty Bottom => GetProperty(nameof(LegendModel.Bottom));

        public ModelFormProperty Font => GetProperty(nameof(LegendModel.Font));

        public ModelFormProperty Bold => GetProperty(nameof(LegendModel.Bold));

        public ModelFormProperty Italic => GetProperty(nameof(LegendModel.Italic));

        public ModelFormProperty Size => GetProperty(nameof(LegendModel.Size));

        public ModelFormProperty Color => GetProperty(nameof(LegendModel.Color));
    }
}
