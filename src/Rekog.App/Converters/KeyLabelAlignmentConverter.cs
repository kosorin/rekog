using Rekog.App.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rekog.App.Converters
{
    public class KeyLabelAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (parameter, value) switch
            {
                ("V", KeyLabelAlignment.TopLeft or KeyLabelAlignment.TopCenter or KeyLabelAlignment.TopRight) => VerticalAlignment.Top,
                ("V", KeyLabelAlignment.CenterLeft or KeyLabelAlignment.Center or KeyLabelAlignment.CenterRight) => VerticalAlignment.Center,
                ("V", KeyLabelAlignment.BottomLeft or KeyLabelAlignment.BottomCenter or KeyLabelAlignment.BottomRight) => VerticalAlignment.Bottom,

                ("H", KeyLabelAlignment.TopLeft or KeyLabelAlignment.CenterLeft or KeyLabelAlignment.BottomLeft) => HorizontalAlignment.Left,
                ("H", KeyLabelAlignment.TopCenter or KeyLabelAlignment.Center or KeyLabelAlignment.BottomCenter) => HorizontalAlignment.Center,
                ("H", KeyLabelAlignment.TopRight or KeyLabelAlignment.CenterRight or KeyLabelAlignment.BottomRight) => HorizontalAlignment.Right,

                ("T", KeyLabelAlignment.TopLeft or KeyLabelAlignment.CenterLeft or KeyLabelAlignment.BottomLeft) => TextAlignment.Left,
                ("T", KeyLabelAlignment.TopCenter or KeyLabelAlignment.Center or KeyLabelAlignment.BottomCenter) => TextAlignment.Center,
                ("T", KeyLabelAlignment.TopRight or KeyLabelAlignment.CenterRight or KeyLabelAlignment.BottomRight) => TextAlignment.Right,

                _ => throw new ArgumentException(null, nameof(value)),
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
