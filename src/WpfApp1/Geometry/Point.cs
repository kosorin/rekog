using System;

namespace WpfApp1.Geometry
{
    public record Point : IComparable<Point>
    {
        public const int DecimalPlaces = 6;

        public Point(double x, double y)
        {
            X = Math.Round(x, DecimalPlaces);
            Y = Math.Round(y, DecimalPlaces);
        }

        public double X { get; }

        public double Y { get; }

        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        public override string ToString()
        {
            return $"[{X};{Y}]";
        }

        public static bool operator <(Point first, Point second)
        {
            return first.CompareTo(second) < 0;
        }

        public static bool operator >(Point first, Point second)
        {
            return first.CompareTo(second) > 0;
        }

        public static bool operator <=(Point first, Point second)
        {
            return first.CompareTo(second) <= 0;
        }

        public static bool operator >=(Point first, Point second)
        {
            return first.CompareTo(second) >= 0;
        }

        public static Point operator +(Point first, Point second)
        {
            return new(first.X + second.X, first.Y + second.Y)!;
        }

        public static Point operator -(Point first, Point second)
        {
            return new(first.X - second.X, first.Y - second.Y)!;
        }

        public double DistanceTo(Point point)
        {
            return Math.Sqrt(Math.Pow(X - point.X, 2) + Math.Pow(Y - point.Y, 2));
        }

        public int CompareTo(Point? other)
        {
            if (other == null)
            {
                return -1;
            }

            var result = Math.Sign(X - other.X);
            if (result == 0)
            {
                result = Math.Sign(Y - other.Y);
            }
            return result;
        }
    }
}
