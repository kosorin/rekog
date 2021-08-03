using System;
using System.Globalization;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(double?), typeof(double))]
    public class NullToNanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? double.NaN;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is double.NaN ? null : value;
        }
    }
}
