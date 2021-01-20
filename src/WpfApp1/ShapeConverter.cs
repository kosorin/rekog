using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1
{
    public class ShapeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(PointCollection))
            {
                throw new NotSupportedException();
            }

            if (values.Length != 2)
            {
                throw new ArgumentException(null, nameof(values));
            }

            var shape = (IList<System.Windows.Point>)values[0];
            var unitSize = (double)values[1];

            return new PointCollection(shape.Select(p => new System.Windows.Point(p.X * unitSize, p.Y * unitSize)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
