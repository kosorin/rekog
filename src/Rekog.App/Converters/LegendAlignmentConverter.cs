using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Rekog.App.Model;

namespace Rekog.App.Converters
{
    public class LegendAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (parameter, value) switch
            {
                ("V", LegendAlignment.TopLeft or LegendAlignment.Top or LegendAlignment.TopRight) => VerticalAlignment.Top,
                ("V", LegendAlignment.Left or LegendAlignment.Center or LegendAlignment.Right) => VerticalAlignment.Center,
                ("V", LegendAlignment.BottomLeft or LegendAlignment.Bottom or LegendAlignment.BottomRight) => VerticalAlignment.Bottom,

                ("H", LegendAlignment.TopLeft or LegendAlignment.Left or LegendAlignment.BottomLeft) => HorizontalAlignment.Left,
                ("H", LegendAlignment.Top or LegendAlignment.Center or LegendAlignment.Bottom) => HorizontalAlignment.Center,
                ("H", LegendAlignment.TopRight or LegendAlignment.Right or LegendAlignment.BottomRight) => HorizontalAlignment.Right,

                ("T", LegendAlignment.TopLeft or LegendAlignment.Left or LegendAlignment.BottomLeft) => TextAlignment.Left,
                ("T", LegendAlignment.Top or LegendAlignment.Center or LegendAlignment.Bottom) => TextAlignment.Center,
                ("T", LegendAlignment.TopRight or LegendAlignment.Right or LegendAlignment.BottomRight) => TextAlignment.Right,

                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
