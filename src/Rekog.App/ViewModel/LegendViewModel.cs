using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Koda.ColorTools;
using Koda.ColorTools.Wpf;
using Rekog.App.Model;

namespace Rekog.App.ViewModel
{
    public class LegendViewModel : ViewModelBase<LegendModel>
    {
        private static readonly Color DefaultColor = Colors.Black;
        private static readonly FontFamily DefaultFont = new FontFamily("Arial");

        private Thickness _margin;
        private Color _color = DefaultColor;
        private FontFamily _font = DefaultFont;

        public LegendViewModel(LegendModel model)
            : base(model)
        {
            UpdateAll();
        }

        public Thickness Margin
        {
            get => _margin;
            private set => Set(ref _margin, value);
        }

        public Color Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public FontFamily Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            base.OnModelPropertyChanged(sender, args);

            switch (args.PropertyName)
            {
                case nameof(LegendModel.Left):
                case nameof(LegendModel.Top):
                case nameof(LegendModel.Right):
                case nameof(LegendModel.Bottom):
                    UpdateMargin();
                    break;
                case nameof(LegendModel.Color):
                    UpdateColor();
                    break;
                case nameof(LegendModel.Font):
                    UpdateFont();
                    break;
            }
        }

        private void UpdateAll()
        {
            UpdateMargin();
            UpdateColor();
            UpdateFont();
        }

        private void UpdateMargin()
        {
            Margin = new Thickness(Model.Left, Model.Top, Model.Right, Model.Bottom);
        }

        private void UpdateColor()
        {
            Color = HexColor.TryParse(Model.Color, out var color) ? color.ToColor() : DefaultColor;
        }

        private void UpdateFont()
        {
            try
            {
                Font = new FontFamily(Model.Font);
            }
            catch
            {
                Font = DefaultFont;
            }
        }
    }
}
