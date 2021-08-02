using Rekog.App.Model;

namespace Rekog.App.ViewModel.Forms
{
    public class KeyFormViewModel : FormViewModel
    {
        public KeyFormViewModel(params KeyModel[] models)
        {
            X = FormProperty.Value(models, x => x.X);
            Y = FormProperty.Value(models, x => x.Y);
            RotationAngle = FormProperty.Value(models, x => x.RotationAngle);
            RotationOriginX = FormProperty.Value(models, x => x.RotationOriginX);
            RotationOriginY = FormProperty.Value(models, x => x.RotationOriginY);
            Shape = FormProperty.NullableReference(models, x => x.Shape);
            SteppedShape = FormProperty.NullableReference(models, x => x.SteppedShape);
            Width = FormProperty.Value(models, x => x.Width);
            Height = FormProperty.Value(models, x => x.Height);
            Color = FormProperty.Reference(models, x => x.Color);
            Roundness = FormProperty.Value(models, x => x.Roundness);
            RoundConcaveCorner = FormProperty.Value(models, x => x.RoundConcaveCorner);
            Margin = FormProperty.Value(models, x => x.Margin);
            Padding = FormProperty.Value(models, x => x.Padding);
            InnerPadding = FormProperty.Value(models, x => x.InnerPadding);
            InnerVerticalOffset = FormProperty.Value(models, x => x.InnerVerticalOffset);
            IsHoming = FormProperty.Value(models, x => x.IsHoming);
            IsGhosted = FormProperty.Value(models, x => x.IsGhosted);
            IsDecal = FormProperty.Value(models, x => x.IsDecal);
        }

        public IFormProperty<double?> X { get; }

        public IFormProperty<double?> Y { get; }

        public IFormProperty<double?> RotationAngle { get; }

        public IFormProperty<double?> RotationOriginX { get; }

        public IFormProperty<double?> RotationOriginY { get; }

        public IFormProperty<string?> Shape { get; }

        public IFormProperty<string?> SteppedShape { get; }

        public IFormProperty<double?> Width { get; }

        public IFormProperty<double?> Height { get; }

        public IFormProperty<string?> Color { get; }

        public IFormProperty<double?> Roundness { get; }

        public IFormProperty<bool?> RoundConcaveCorner { get; }

        public IFormProperty<double?> Margin { get; }

        public IFormProperty<double?> Padding { get; }

        public IFormProperty<double?> InnerPadding { get; }

        public IFormProperty<double?> InnerVerticalOffset { get; }

        public IFormProperty<bool?> IsHoming { get; }

        public IFormProperty<bool?> IsGhosted { get; }

        public IFormProperty<bool?> IsDecal { get; }
    }
}
