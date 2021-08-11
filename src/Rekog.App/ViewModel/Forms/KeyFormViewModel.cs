using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class KeyFormViewModel : FormViewModel<KeyModel>
    {
        public KeyFormViewModel()
        {
            Properties = new FormProperty<KeyModel>[]
            {
                X = FormProperty<KeyModel>.Value(this, x => x.X),
                Y = FormProperty<KeyModel>.Value(this, x => x.Y),
                RotationAngle = FormProperty<KeyModel>.Value(this, x => x.RotationAngle),
                RotationOriginX = FormProperty<KeyModel>.Value(this, x => x.RotationOriginX),
                RotationOriginY = FormProperty<KeyModel>.Value(this, x => x.RotationOriginY),
                Width = FormProperty<KeyModel>.Value(this, x => x.Width),
                Height = FormProperty<KeyModel>.Value(this, x => x.Height),
                UseShape = FormProperty<KeyModel>.Value(this, x => x.UseShape),
                Shape = FormProperty<KeyModel>.NullableReference(this, x => x.Shape),
                IsStepped = FormProperty<KeyModel>.Value(this, x => x.IsStepped),
                SteppedOffsetX = FormProperty<KeyModel>.Value(this, x => x.SteppedOffsetX),
                SteppedOffsetY = FormProperty<KeyModel>.Value(this, x => x.SteppedOffsetY),
                SteppedWidth = FormProperty<KeyModel>.Value(this, x => x.SteppedWidth),
                SteppedHeight = FormProperty<KeyModel>.Value(this, x => x.SteppedHeight),
                UseSteppedShape = FormProperty<KeyModel>.Value(this, x => x.UseSteppedShape),
                SteppedShape = FormProperty<KeyModel>.NullableReference(this, x => x.SteppedShape),
                Color = FormProperty<KeyModel>.Reference(this, x => x.Color),
                Roundness = FormProperty<KeyModel>.Value(this, x => x.Roundness),
                RoundConcaveCorner = FormProperty<KeyModel>.Value(this, x => x.RoundConcaveCorner),
                Margin = FormProperty<KeyModel>.Value(this, x => x.Margin),
                Padding = FormProperty<KeyModel>.Value(this, x => x.Padding),
                InnerPadding = FormProperty<KeyModel>.Value(this, x => x.InnerPadding),
                InnerVerticalOffset = FormProperty<KeyModel>.Value(this, x => x.InnerVerticalOffset),
                IsHoming = FormProperty<KeyModel>.Value(this, x => x.IsHoming),
                IsGhosted = FormProperty<KeyModel>.Value(this, x => x.IsGhosted),
                IsDecal = FormProperty<KeyModel>.Value(this, x => x.IsDecal),
            };
        }

        public override IReadOnlyCollection<FormProperty<KeyModel>> Properties { get; }

        public FormProperty<KeyModel, double?> X { get; }

        public FormProperty<KeyModel, double?> Y { get; }

        public FormProperty<KeyModel, double?> RotationAngle { get; }

        public FormProperty<KeyModel, double?> RotationOriginX { get; }

        public FormProperty<KeyModel, double?> RotationOriginY { get; }

        public FormProperty<KeyModel, double?> Width { get; }

        public FormProperty<KeyModel, double?> Height { get; }

        public FormProperty<KeyModel, bool?> UseShape { get; }

        public FormProperty<KeyModel, string?> Shape { get; }

        public FormProperty<KeyModel, bool?> IsStepped { get; }

        public FormProperty<KeyModel, double?> SteppedOffsetX { get; }

        public FormProperty<KeyModel, double?> SteppedOffsetY { get; }

        public FormProperty<KeyModel, double?> SteppedWidth { get; }

        public FormProperty<KeyModel, double?> SteppedHeight { get; }

        public FormProperty<KeyModel, bool?> UseSteppedShape { get; }

        public FormProperty<KeyModel, string?> SteppedShape { get; }

        public FormProperty<KeyModel, string?> Color { get; }

        public FormProperty<KeyModel, double?> Roundness { get; }

        public FormProperty<KeyModel, bool?> RoundConcaveCorner { get; }

        public FormProperty<KeyModel, double?> Margin { get; }

        public FormProperty<KeyModel, double?> Padding { get; }

        public FormProperty<KeyModel, double?> InnerPadding { get; }

        public FormProperty<KeyModel, double?> InnerVerticalOffset { get; }

        public FormProperty<KeyModel, bool?> IsHoming { get; }

        public FormProperty<KeyModel, bool?> IsGhosted { get; }

        public FormProperty<KeyModel, bool?> IsDecal { get; }
    }
}
