using System;

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

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public KeyLabelAlignment Alignment
        {
            get => _alignment;
            set => Set(ref _alignment, value);
        }

        public double Left
        {
            get => _left;
            set => Set(ref _left, Math.Round(value, HighPrecision));
        }

        public double Top
        {
            get => _top;
            set => Set(ref _top, Math.Round(value, HighPrecision));
        }

        public double Right
        {
            get => _right;
            set => Set(ref _right, Math.Round(value, HighPrecision));
        }

        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, Math.Round(value, HighPrecision));
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

        public double Size
        {
            get => _size;
            set => Set(ref _size, Math.Round(value, Precision));
        }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }
    }
}
