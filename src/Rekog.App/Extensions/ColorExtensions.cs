using System.Text;
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

        public static string ToHex(this Color color)
        {
            var sb = new StringBuilder(9);

            sb.Append('#');

            if (color.A != 255)
            {
                sb.Append(color.A.ToString("X2"));
            }

            sb.Append(color.R.ToString("X2"));
            sb.Append(color.G.ToString("X2"));
            sb.Append(color.B.ToString("X2"));

            return sb.ToString();
        }
    }
}
