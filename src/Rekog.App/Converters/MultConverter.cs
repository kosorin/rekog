using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    public class MultConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Aggregate(1d, (a, x) => a * (x is double value ? value : 1d));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
