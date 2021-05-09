using Rekog.App.Converters;
using System;
using System.Windows;

namespace Rekog.App.Model
{
    public class KeyLabelModel : ModelBase
    {
        private double _left;
        private double _top;
        private double _right;
        private double _bottom;
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;
        private double _size = 20;
        private string _color = "#000000";
        private string _font = "Arial";
        private bool _bold;
        private bool _italic;
        private string _value = string.Empty;

        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.Spinnable(1, 10, -1000, 1000)]
        public double Left
        {
            get => _left;
            set => Set(ref _left, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.Spinnable(1, 10, -1000, 1000)]
        public double Top
        {
            get => _top;
            set => Set(ref _top, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.Spinnable(1, 10, -1000, 1000)]
        public double Right
        {
            get => _right;
            set => Set(ref _right, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.Spinnable(1, 10, -1000, 1000)]
        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Category("Alignment")]
        public HorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => Set(ref _horizontalAlignment, value);
        }

        [PropertyTools.DataAnnotations.Category("Alignment")]
        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set => Set(ref _verticalAlignment, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.Spinnable(0.5, 1, 4, 96)]
        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.Converter(typeof(HexToColorConverter))]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.FontFamilySelector]
        [PropertyTools.DataAnnotations.FontPreview(24)]
        public string Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        public bool Bold
        {
            get => _bold;
            set => Set(ref _bold, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        public bool Italic
        {
            get => _italic;
            set => Set(ref _italic, value);
        }

        [PropertyTools.DataAnnotations.Category("Value")]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}
