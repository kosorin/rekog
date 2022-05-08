namespace Rekog.App.Model
{
    public record LegendId(KeyId KeyId, LayerId LayerId)
    {
        public static implicit operator LegendId((KeyId keyId, LayerId layerId) value)
        {
            return new LegendId(value.keyId, value.layerId);
        }
    }

    public class LegendModel : ModelBase
    {
        private string _value = string.Empty;
        private LegendAlignment _alignment;
        private double _left;
        private double _top;
        private double _right;
        private double _bottom;
        private string _font = "Arial";
        private bool _bold;
        private bool _italic;
        private double _size = 20;
        private string _color = "#000000";

        public LegendModel(LegendId id)
        {
            Id = id;
        }

        public LegendId Id { get; }

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public LegendAlignment Alignment
        {
            get => _alignment;
            set => Set(ref _alignment, value);
        }

        public double Left
        {
            get => _left;
            set => Set(ref _left, value);
        }

        public double Top
        {
            get => _top;
            set => Set(ref _top, value);
        }

        public double Right
        {
            get => _right;
            set => Set(ref _right, value);
        }

        public double Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, value);
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
            set => Set(ref _size, value);
        }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }
    }
}
