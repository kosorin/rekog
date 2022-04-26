using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class LegendFormViewModel : FormViewModel<LegendModel>
    {
        public LegendFormViewModel()
        {
            Properties = new FormProperty<LegendModel>[]
            {
                Value = FormProperty.Reference(this, x => x.Value),
                Alignment = FormProperty.Value(this, x => x.Alignment),
                Left = FormProperty.Value(this, x => x.Left),
                Top = FormProperty.Value(this, x => x.Top),
                Right = FormProperty.Value(this, x => x.Right),
                Bottom = FormProperty.Value(this, x => x.Bottom),
                Font = FormProperty.Reference(this, x => x.Font),
                Bold = FormProperty.Value(this, x => x.Bold),
                Italic = FormProperty.Value(this, x => x.Italic),
                Size = FormProperty.Value(this, x => x.Size),
                Color = FormProperty.Reference(this, x => x.Color),
            };
        }

        public override IReadOnlyCollection<FormProperty<LegendModel>> Properties { get; }

        public FormProperty<LegendModel, string?> Value { get; }

        public FormProperty<LegendModel, LegendAlignment?> Alignment { get; }

        public FormProperty<LegendModel, double?> Left { get; }

        public FormProperty<LegendModel, double?> Top { get; }

        public FormProperty<LegendModel, double?> Right { get; }

        public FormProperty<LegendModel, double?> Bottom { get; }

        public FormProperty<LegendModel, string?> Font { get; }

        public FormProperty<LegendModel, bool?> Bold { get; }

        public FormProperty<LegendModel, bool?> Italic { get; }

        public FormProperty<LegendModel, double?> Size { get; }

        public FormProperty<LegendModel, string?> Color { get; }
    }
}
