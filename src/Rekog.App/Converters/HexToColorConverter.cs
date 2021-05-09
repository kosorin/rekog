using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    public class HexToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                null => null,
                string hex => (Color)ColorConverter.ConvertFromString(hex),
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                null => null,
                Color color => ToHex(color),
                _ => throw new ArgumentException(null, nameof(value)),
            };

            string ToHex(Color color)
            {
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
        }
    }
}
