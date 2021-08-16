using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    public class EnumIsInConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            return values.Length > 0
                && values.First() is Enum valueToCheck
                && values.Skip(1).Contains(valueToCheck);
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
