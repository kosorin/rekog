using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Rekog.App.Extensions;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(Geometry), typeof(PathGeometry), ParameterType = typeof(double))]
    public class EnlargedGeometryConverter : IValueConverter
    {
        public bool Round { get; init; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Geometry geometry)
            {
                return DependencyProperty.UnsetValue;
            }
            if (parameter is not double size)
            {
                return geometry;
            }

            try
            {
                return geometry.GetEnlargedPathGeometry(size, Round);
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
    }
}
