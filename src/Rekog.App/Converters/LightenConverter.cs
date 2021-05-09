using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Rekog.App.Extensions;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(Color), typeof(Color), ParameterType = typeof(double?))]
    public class LightenConverter : IValueConverter
    {
        public double Amount { get; set; } = 1;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value, parameter) switch
            {
                (Color color, double amount) => color.Lighten(amount),
                (Color color, _) => color.Lighten(Amount),
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
