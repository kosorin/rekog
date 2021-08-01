using System;
using System.Globalization;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NotNullToBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
