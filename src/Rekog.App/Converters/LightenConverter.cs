using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Rekog.App.Extensions;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(Color), typeof(Color), ParameterType = typeof(double?))]
    public class LightenConverter : IValueConverter
    {
        public double Amount { get; } = 1;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                return (value, parameter) switch
                {
                    (Color color, double amount) => color.Lighten(amount),
                    (Color color, _) => color.Lighten(Amount),
                    _ => DependencyProperty.UnsetValue,
                };
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
