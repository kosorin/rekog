using Rekog.App.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

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
                return null;
            }
            if (parameter is not double size)
            {
                return geometry;
            }
            return geometry.GetEnlargedPathGeometry(size, Round);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
