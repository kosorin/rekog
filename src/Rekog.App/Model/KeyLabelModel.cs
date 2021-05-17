using Rekog.App.Converters;
using System;
using System.Windows;

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

        [PropertyTools.DataAnnotations.Category("Value")]
        [PropertyTools.DataAnnotations.SortIndex(0)]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        [PropertyTools.DataAnnotations.Category("Alignment")]
        [PropertyTools.DataAnnotations.SortIndex(11)]
        public KeyLabelAlignment Alignment
        {
            get => _alignment;
            set => Set(ref _alignment, value);
        }

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

        [PropertyTools.DataAnnotations.Category("Style")]
        [PropertyTools.DataAnnotations.FontFamilySelector]
        [PropertyTools.DataAnnotations.FontPreview(24)]
        [PropertyTools.DataAnnotations.SortIndex(31)]
        public string Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        [PropertyTools.DataAnnotations.Category("Style")]
        [PropertyTools.DataAnnotations.SortIndex(32)]
        public bool Bold
        {
            get => _bold;
            set => Set(ref _bold, value);
        }

        [PropertyTools.DataAnnotations.Category("Style")]
        [PropertyTools.DataAnnotations.SortIndex(33)]
        public bool Italic
        {
            get => _italic;
            set => Set(ref _italic, value);
        }

        [PropertyTools.DataAnnotations.Category("Style")]
        [PropertyTools.DataAnnotations.Spinnable(0.5, 1, 4, 96)]
        [PropertyTools.DataAnnotations.SortIndex(34)]
        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, Precision));
        }

        [PropertyTools.DataAnnotations.Category("Style")]
        [PropertyTools.DataAnnotations.Converter(typeof(StringToColorConverter))]
        [PropertyTools.DataAnnotations.SortIndex(35)]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }
    }
}
