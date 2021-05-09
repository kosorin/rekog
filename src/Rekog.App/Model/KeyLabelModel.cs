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

        public double Left
        {
            get => _left;
            set => Set(ref _left, Math.Round(value, DoublePrecision));
        }

        public double Top
        {
            get => _top;
            set => Set(ref _top, Math.Round(value, DoublePrecision));
        }

        public double Right
        {
            get => _right;
            set => Set(ref _right, Math.Round(value, DoublePrecision));
        }

        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, Math.Round(value, DoublePrecision));
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => Set(ref _horizontalAlignment, value);
        }

        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set => Set(ref _verticalAlignment, value);
        }

        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, DoublePrecision));
        }

        [PropertyTools.DataAnnotations.Converter(typeof(HexToColorConverter))]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public string Font
        {
            get => _font;
            set => Set(ref _font, value);
        }

        public bool Bold
        {
            get => _bold;
            set => Set(ref _bold, value);
        }

        public bool Italic
        {
            get => _italic;
            set => Set(ref _italic, value);
        }

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}
