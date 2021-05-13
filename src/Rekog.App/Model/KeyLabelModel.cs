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

        [PropertyTools.DataAnnotations.Spinnable(0.01, 0.1, -10, 10)]
        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.SortIndex(21)]
        public double Left
        {
            get => _left;
            set => Set(ref _left, Math.Round(value, HighPrecision));
        }

        [PropertyTools.DataAnnotations.Spinnable(0.01, 0.1, -10, 10)]
        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.SortIndex(22)]
        public double Top
        {
            get => _top;
            set => Set(ref _top, Math.Round(value, HighPrecision));
        }

        [PropertyTools.DataAnnotations.Spinnable(0.01, 0.1, -10, 10)]
        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.SortIndex(23)]
        public double Right
        {
            get => _right;
            set => Set(ref _right, Math.Round(value, HighPrecision));
        }

        [PropertyTools.DataAnnotations.Spinnable(0.01, 0.1, -10, 10)]
        [PropertyTools.DataAnnotations.Category("Offset")]
        [PropertyTools.DataAnnotations.SortIndex(24)]
        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, Math.Round(value, HighPrecision));
        }

        [PropertyTools.DataAnnotations.Category("Alignment")]
        [PropertyTools.DataAnnotations.SortIndex(11)]
        public HorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => Set(ref _horizontalAlignment, value);
        }

        [PropertyTools.DataAnnotations.Category("Alignment")]
        [PropertyTools.DataAnnotations.SortIndex(12)]
        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set => Set(ref _verticalAlignment, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.Spinnable(0.5, 1, 4, 96)]
        [PropertyTools.DataAnnotations.SortIndex(2)]
        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, Precision));
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.Converter(typeof(StringToColorConverter))]
        [PropertyTools.DataAnnotations.SortIndex(5)]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.FontFamilySelector]
        [PropertyTools.DataAnnotations.FontPreview(24)]
        [PropertyTools.DataAnnotations.SortIndex(1)]
        public string Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.SortIndex(3)]
        public bool Bold
        {
            get => _bold;
            set => Set(ref _bold, value);
        }

        [PropertyTools.DataAnnotations.Category("Font")]
        [PropertyTools.DataAnnotations.SortIndex(4)]
        public bool Italic
        {
            get => _italic;
            set => Set(ref _italic, value);
        }

        [PropertyTools.DataAnnotations.Category("Value")]
        [PropertyTools.DataAnnotations.SortIndex(0)]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}
