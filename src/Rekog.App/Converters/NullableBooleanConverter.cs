using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NullableBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value, parameter) switch
            {
                (bool v, _) => v,
                (null, bool p) => p,
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
