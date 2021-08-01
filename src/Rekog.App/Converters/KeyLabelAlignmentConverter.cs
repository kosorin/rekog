using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Rekog.App.Model;

namespace Rekog.App.Converters
{
    public class KeyLabelAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (parameter, value) switch
            {
                ("V", KeyLabelAlignment.TopLeft or KeyLabelAlignment.Top or KeyLabelAlignment.TopRight) => VerticalAlignment.Top,
                ("V", KeyLabelAlignment.Left or KeyLabelAlignment.Center or KeyLabelAlignment.Right) => VerticalAlignment.Center,
                ("V", KeyLabelAlignment.BottomLeft or KeyLabelAlignment.Bottom or KeyLabelAlignment.BottomRight) => VerticalAlignment.Bottom,

                ("H", KeyLabelAlignment.TopLeft or KeyLabelAlignment.Left or KeyLabelAlignment.BottomLeft) => HorizontalAlignment.Left,
                ("H", KeyLabelAlignment.Top or KeyLabelAlignment.Center or KeyLabelAlignment.Bottom) => HorizontalAlignment.Center,
                ("H", KeyLabelAlignment.TopRight or KeyLabelAlignment.Right or KeyLabelAlignment.BottomRight) => HorizontalAlignment.Right,

                ("T", KeyLabelAlignment.TopLeft or KeyLabelAlignment.Left or KeyLabelAlignment.BottomLeft) => TextAlignment.Left,
                ("T", KeyLabelAlignment.Top or KeyLabelAlignment.Center or KeyLabelAlignment.Bottom) => TextAlignment.Center,
                ("T", KeyLabelAlignment.TopRight or KeyLabelAlignment.Right or KeyLabelAlignment.BottomRight) => TextAlignment.Right,

                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
