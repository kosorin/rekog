using Rekog.App.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    public class StringToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                null => null,
                string s => s.ToColor(),
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                null => null,
                Color c => c.ToHex(),
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }
    }
}
