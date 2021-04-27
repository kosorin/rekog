using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    public class ContainsConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return false;
            }

            if (values[0] is not { } value)
            {
                return false;
            }

            var enumType = value.GetType();
            return enumType.IsEnum
                && values.All(x => x?.GetType() == enumType)
                && values.Skip(1).Contains(values.First());
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
