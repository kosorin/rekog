using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value, parameter) switch
            {
                (true, _) => Visibility.Visible,
                (false, Visibility visibility) => visibility,
                (false, _) => Visibility.Collapsed,
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                Visibility.Visible => true,
                Visibility.Collapsed or Visibility.Hidden => false,
                _ => DependencyProperty.UnsetValue,
            };
        }
    }
}
