using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(bool), typeof(FontStyle))]
    public class BooleanToFontStyleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool italic
                ? italic ? FontStyles.Italic : FontStyles.Normal
                : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is FontStyle fontStyle
                ? fontStyle == FontStyles.Italic
                : DependencyProperty.UnsetValue;
        }
    }
}
