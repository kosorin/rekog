using System;
using PropertyTools.DataAnnotations;
using Rekog.App.Converters;

namespace Rekog.App.Model
{
    public class KeyLabelModel : ModelBase
    {
        private string _value = string.Empty;
        private KeyLabelAlignment _alignment;
        private double _left;
        private double _top;
        private double _right;
        private double _bottom;
        private string _font = "Arial";
        private bool _bold;
        private bool _italic;
        private double _size = 20;
        private string _color = "#000000";

        [Category("Value")]
        [SortIndex(0)]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        [Category("Alignment")]
        [SortIndex(11)]
        public KeyLabelAlignment Alignment
        {
            get => _alignment;
            set => Set(ref _alignment, value);
        }

        [Spinnable(0.01, 0.1, -10, 10)]
        [Category("Offset")]
        [SortIndex(21)]
        public double Left
        {
            get => _left;
            set => Set(ref _left, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.01, 0.1, -10, 10)]
        [Category("Offset")]
        [SortIndex(22)]
        public double Top
        {
            get => _top;
            set => Set(ref _top, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.01, 0.1, -10, 10)]
        [Category("Offset")]
        [SortIndex(23)]
        public double Right
        {
            get => _right;
            set => Set(ref _right, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.01, 0.1, -10, 10)]
        [Category("Offset")]
        [SortIndex(24)]
        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, Math.Round(value, HighPrecision));
        }

        [Category("Style")]
        [FontFamilySelector]
        [FontPreview(24)]
        [SortIndex(31)]
        public string Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        [Category("Style")]
        [SortIndex(32)]
        public bool Bold
        {
            get => _bold;
            set => Set(ref _bold, value);
        }

        [Category("Style")]
        [SortIndex(33)]
        public bool Italic
        {
            get => _italic;
            set => Set(ref _italic, value);
        }

        [Category("Style")]
        [Spinnable(0.5, 1, 4, 96)]
        [SortIndex(34)]
        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, Precision));
        }

        [Category("Style")]
        [Converter(typeof(StringToColorConverter))]
        [SortIndex(35)]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }
    }
}
