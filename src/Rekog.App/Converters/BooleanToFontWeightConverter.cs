using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(bool), typeof(FontWeight))]
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool bold
                ? bold ? FontWeights.Bold : FontWeights.Normal
                : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is FontWeight fontWeights
                ? fontWeights == FontWeights.Bold
                : DependencyProperty.UnsetValue;
        }
    }
}
