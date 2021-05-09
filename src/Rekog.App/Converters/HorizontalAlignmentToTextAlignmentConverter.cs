using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(HorizontalAlignment), typeof(TextAlignment))]
    public class HorizontalAlignmentToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                HorizontalAlignment.Left => TextAlignment.Left,
                HorizontalAlignment.Center => TextAlignment.Center,
                HorizontalAlignment.Right => TextAlignment.Right,
                HorizontalAlignment.Stretch => TextAlignment.Justify,
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                TextAlignment.Left => HorizontalAlignment.Left,
                TextAlignment.Center => HorizontalAlignment.Center,
                TextAlignment.Right => HorizontalAlignment.Right,
                TextAlignment.Justify => HorizontalAlignment.Stretch,
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }
    }
}
