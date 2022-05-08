using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class KeyModelForm : ModelForm<KeyModel>
    {
        public KeyModelForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty X => GetProperty(nameof(KeyModel.X));

        public ModelFormProperty Y => GetProperty(nameof(KeyModel.Y));

        public ModelFormProperty RotationAngle => GetProperty(nameof(KeyModel.RotationAngle));

        public ModelFormProperty RotationOriginX => GetProperty(nameof(KeyModel.RotationOriginX));

        public ModelFormProperty RotationOriginY => GetProperty(nameof(KeyModel.RotationOriginY));

        public ModelFormProperty Width => GetProperty(nameof(KeyModel.Width));

        public ModelFormProperty Height => GetProperty(nameof(KeyModel.Height));

        public ModelFormProperty UseShape => GetProperty(nameof(KeyModel.UseShape));

        public ModelFormProperty Shape => GetProperty(nameof(KeyModel.Shape));

        public ModelFormProperty IsStepped => GetProperty(nameof(KeyModel.IsStepped));

        public ModelFormProperty SteppedOffsetX => GetProperty(nameof(KeyModel.SteppedOffsetX));

        public ModelFormProperty SteppedOffsetY => GetProperty(nameof(KeyModel.SteppedOffsetY));

        public ModelFormProperty SteppedWidth => GetProperty(nameof(KeyModel.SteppedWidth));

        public ModelFormProperty SteppedHeight => GetProperty(nameof(KeyModel.SteppedHeight));

        public ModelFormProperty UseSteppedShape => GetProperty(nameof(KeyModel.UseSteppedShape));

        public ModelFormProperty SteppedShape => GetProperty(nameof(KeyModel.SteppedShape));

        public ModelFormProperty Color => GetProperty(nameof(KeyModel.Color));

        public ModelFormProperty Roundness => GetProperty(nameof(KeyModel.Roundness));

        public ModelFormProperty RoundConcaveCorner => GetProperty(nameof(KeyModel.RoundConcaveCorner));

        public ModelFormProperty Margin => GetProperty(nameof(KeyModel.Margin));

        public ModelFormProperty Padding => GetProperty(nameof(KeyModel.Padding));

        public ModelFormProperty InnerPadding => GetProperty(nameof(KeyModel.InnerPadding));

        public ModelFormProperty InnerVerticalOffset => GetProperty(nameof(KeyModel.InnerVerticalOffset));

        public ModelFormProperty IsHoming => GetProperty(nameof(KeyModel.IsHoming));

        public ModelFormProperty IsGhosted => GetProperty(nameof(KeyModel.IsGhosted));

        public ModelFormProperty IsDecal => GetProperty(nameof(KeyModel.IsDecal));
    }
}
