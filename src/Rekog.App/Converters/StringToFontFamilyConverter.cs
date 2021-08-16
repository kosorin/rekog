using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(FontFamily), typeof(string))]
    public class StringToFontFamilyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                FontFamily fontFamily => fontFamily,
                string source => Fonts.SystemFontFamilies.FirstOrDefault(x => x.Source == source),
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                FontFamily { Source: var source, } => source,
                string source => source,
                _ => DependencyProperty.UnsetValue,
            };
        }
    }
}
