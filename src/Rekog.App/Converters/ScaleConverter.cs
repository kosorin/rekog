using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rekog.App.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public double Size { get; init; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var scale = parameter is double s ? s : 1d;

            switch (value)
            {
            case double v: return ConvertDouble(v, scale, targetType);
            case Thickness v: return ConvertThickness(v, scale, targetType);
            case PointCollection v: return ConvertPointCollection(v, scale, targetType);
            case Geometry v: return ConvertGeometry(v, scale, targetType);
            default: throw new NotSupportedException();
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private object ConvertDouble(double value, double scale, Type targetType)
        {
            if (targetType == typeof(double))
            {
                return value * scale * Size;
            }
            else if (targetType == typeof(Thickness))
            {
                return new Thickness(value * scale * Size);
            }

            throw new NotSupportedException();
        }

        private object ConvertThickness(Thickness value, double scale, Type targetType)
        {
            if (targetType == typeof(Thickness))
            {
                return new Thickness(value.Left * scale * Size, value.Top * scale * Size, value.Right * scale * Size, value.Bottom * scale * Size);
            }

            throw new NotSupportedException();
        }

        private object ConvertPointCollection(PointCollection value, double scale, Type targetType)
        {
            if (targetType == typeof(PointCollection))
            {
                return new PointCollection(value
                    .Cast<Point>()
                    .Select(p => new Point(p.X * scale * Size, p.Y * scale * Size)));
            }

            throw new NotSupportedException();
        }

        private object ConvertGeometry(Geometry value, double scale, Type targetType)
        {
            if (targetType == typeof(Geometry))
            {
                var scaleTransform = new ScaleTransform(scale * Size, scale * Size);

                var geometry = value.Clone();
                if (geometry.Transform is Transform transform)
                {
                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(transform);
                    transformGroup.Children.Add(scaleTransform);
                    geometry.Transform = transformGroup;
                }
                else
                {
                    geometry.Transform = scaleTransform;
                }
                return geometry;
            }

            throw new NotSupportedException();
        }
    }
}
