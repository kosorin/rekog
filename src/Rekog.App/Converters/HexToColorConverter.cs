using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Koda.ColorTools;
using Koda.ColorTools.Wpf;

namespace Rekog.App.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    public class HexToColorConverter : IValueConverter
    {
        public bool WithAlpha { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var color = value is string hex && HexColor.TryParse(hex, out var c)
                ? c.ToColor()
                : parameter is Color dc
                    ? dc
                    : (Color?)null;

            return color ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var color = value is Color c
                ? c
                : parameter is Color dc
                    ? dc
                    : (Color?)null;

            return color is { A: var a, R: var r, G: var g, B: var b, }
                ? new HexColor(a, r, g, b).ToString(WithAlpha)
                : DependencyProperty.UnsetValue;
        }
    }
}
