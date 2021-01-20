using System;
using System.Linq;
using WpfApp1.Geometry;

namespace WpfApp1
{
    public class KleKey
    {
        public string?[] Labels { get; set; } = Array.Empty<string?>();


        public double RotationAngle { get; set; }

        public double RotationX { get; set; }

        public double RotationY { get; set; }


        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        public double Width2 { get; set; }

        public double Height2 { get; set; }


        public string BackgroundColor { get; set; } = string.Empty;

        public string DefaultTextColor { get; set; } = string.Empty;

        public int DefaultTextSize { get; set; }

        public string?[] TextColors { get; set; } = Array.Empty<string?>();

        public int?[] TextSizes { get; set; } = Array.Empty<int?>();


        public bool IsHoming { get; set; }

        public bool IsStepped { get; set; }

        public bool IsDecal { get; set; }

        public bool IsGhosted { get; set; }


        public Polygon GetPrimaryPolygon()
        {
            var polygon1 = new Polygon(new Point[]
            {
                new(X, Y),
                new(X + Width, Y),
                new(X + Width, Y + Height),
                new(X, Y + Height),
                new(X, Y),
            });

            if (X2 == 0 && Y2 == 0 && Width == Width2 && Height == Height2)
            {
                return polygon1;
            }

            var polygon2 = new Polygon(new Point[]
            {
                new(X + X2, Y + Y2),
                new(X + X2 + Width2, Y + Y2),
                new(X + X2 + Width2, Y + Y2 + Height2),
                new(X + X2, Y + Y2 + Height2),
                new(X + X2, Y + Y2),
            });

            return polygon1.Union(polygon2).First();
        }

        public Polygon GetSecondaryPolygon()
        {
            if (IsStepped)
            {
                return new Polygon(new Point[]
                {
                    new(X, Y),
                    new(X + Width, Y),
                    new(X + Width, Y + Height),
                    new(X, Y + Height),
                    new(X, Y),
                });
            }

            return GetPrimaryPolygon();
        }

        [Obsolete]
        public Polygon[] GetPolygons()
        {
            var polygon1 = new Polygon(new Point[]
            {
                new(X, Y),
                new(X + Width, Y),
                new(X + Width, Y + Height),
                new(X, Y + Height),
                new(X, Y),
            });

            if (X2 == 0 && Y2 == 0 && Width == Width2 && Height == Height2)
            {
                return new[] { polygon1 };
            }

            var polygon2 = new Polygon(new Point[]
            {
                new(X + X2, Y + Y2),
                new(X + X2 + Width2, Y + Y2),
                new(X + X2 + Width2, Y + Y2 + Height2),
                new(X + X2, Y + Y2 + Height2),
                new(X + X2, Y + Y2),
            });

            return new[] { polygon1, polygon2 };
        }
    }
}
