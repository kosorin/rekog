using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(double), typeof(double), ParameterType = typeof(double))]
    [ValueConversion(typeof(double), typeof(Thickness), ParameterType = typeof(double))]
    [ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(double))]
    [ValueConversion(typeof(PointCollection), typeof(PointCollection), ParameterType = typeof(double))]
    [ValueConversion(typeof(Geometry), typeof(Geometry), ParameterType = typeof(double))]
    public class ScaleConverter : IValueConverter
    {
        public double Scale { get; init; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var scale = (parameter is double s ? s : 1d) * Scale;

            try
            {
                return value switch
                {
                    double v when targetType == typeof(Thickness) => ConvertThickness(v, scale),
                    double v => ConvertDouble(v, scale),
                    Thickness v => ConvertThickness(v, scale),
                    PointCollection v => ConvertPointCollection(v, scale),
                    Geometry v => ConvertGeometry(v, scale),
                    _ => DependencyProperty.UnsetValue,
                };
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private object ConvertDouble(double value, double scale)
        {
            return value * scale;
        }

        private object ConvertThickness(double value, double scale)
        {
            return new Thickness(value * scale);
        }

        private object ConvertThickness(Thickness value, double scale)
        {
            return new Thickness(value.Left * scale, value.Top * scale, value.Right * scale, value.Bottom * scale);
        }

        private object ConvertPointCollection(PointCollection value, double scale)
        {
            return new PointCollection(value.Select(p => new Point(p.X * scale, p.Y * scale)));
        }

        private object ConvertGeometry(Geometry value, double scale)
        {
            var scaleTransform = new ScaleTransform(scale, scale);

            var geometry = value.Clone();
            if (geometry.Transform is { } transform)
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
    }
}
