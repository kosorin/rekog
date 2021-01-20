using GalaSoft.MvvmLight;
using System.Collections.Generic;
using WpfApp1.Geometry;

namespace WpfApp1.Layout
{
    public class Key : ObservableObject
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }


        public double RotationAngle { get; set; }

        public double RotationOriginX { get; set; }

        public double RotationOriginY { get; set; }

        public List<(double x, double y)> RotatedShape { get; set; } = new();


        public bool IsHoming { get; set; }

        public string Color { get; set; } = string.Empty;

        public List<KeyLabel> Labels { get; set; } = new();


        public bool IsDecal { get; set; }

        public bool IsGhosted { get; set; }
    }

    public class KeyShape
    {
        public List<Point> Shape { get; set; } = new();

        public List<Point>? SteppedShape { get; set; } = new();
    }

    public class KeyLabel
    {
        public int Position { get; set; }

        public int Size { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
