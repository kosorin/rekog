using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class LegendFormViewModel : FormViewModel
    {
        public LegendFormViewModel(params LegendModel[] models) : base(models)
        {
            Value = FormProperty.Reference(models, x => x.Value);
            Alignment = FormProperty.Value(models, x => x.Alignment);
            Left = FormProperty.Value(models, x => x.Left);
            Top = FormProperty.Value(models, x => x.Top);
            Right = FormProperty.Value(models, x => x.Right);
            Bottom = FormProperty.Value(models, x => x.Bottom);
            Font = FormProperty.Reference(models, x => x.Font);
            Bold = FormProperty.Value(models, x => x.Bold);
            Italic = FormProperty.Value(models, x => x.Italic);
            Size = FormProperty.Value(models, x => x.Size);
            Color = FormProperty.Reference(models, x => x.Color);
        }

        public IFormProperty<string?> Value { get; }

        public IFormProperty<LegendAlignment?> Alignment { get; }

        public IFormProperty<double?> Left { get; }

        public IFormProperty<double?> Top { get; }

        public IFormProperty<double?> Right { get; }

        public IFormProperty<double?> Bottom { get; }

        public IFormProperty<string?> Font { get; }

        public IFormProperty<bool?> Bold { get; }

        public IFormProperty<bool?> Italic { get; }

        public IFormProperty<double?> Size { get; }

        public IFormProperty<string?> Color { get; }
    }
}
