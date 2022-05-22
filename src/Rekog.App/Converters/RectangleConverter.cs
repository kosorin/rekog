using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    public class RectangleConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 || values.Any(x => x is not double))
            {
                return new Rect();
            }
            return new Rect((double)values[0]!, (double)values[1]!, (double)values[2]!, (double)values[3]!);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
