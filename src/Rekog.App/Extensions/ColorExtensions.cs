using System.Windows.Media;
using Rekog.App.Core;

namespace Rekog.App.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lighten(this Color color, double amount)
        {
            return new HslColor(color).Lighten(amount).ToRgb();
        }
    }
}
