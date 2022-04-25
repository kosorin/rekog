using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(object), typeof(IList))]
    public class ObjectToListConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return new[] { value, };
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList { Count: > 0, } list)
            {
                return list[0];
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
