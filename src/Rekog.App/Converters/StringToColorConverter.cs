using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Rekog.App.Extensions;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    [ValueConversion(typeof(string), typeof(Brush))]
    public class StringToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                return value switch
                {
                    null => null,
                    string s when targetType == typeof(Color) => s.ToColor(),
                    string s when targetType == typeof(Brush) => new SolidColorBrush(s.ToColor()),
                    _ => DependencyProperty.UnsetValue,
                };
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                return value switch
                {
                    null => null,
                    Color c => c.ToHex(),
                    SolidColorBrush { Color: var c, } => c.ToHex(),
                    _ => DependencyProperty.UnsetValue,
                };
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
