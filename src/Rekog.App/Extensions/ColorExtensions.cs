using System;
using System.Globalization;
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

        public static Color ToColor(this string hex, bool withAlpha = false, Color? defaultColor = null)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return defaultColor ?? throw new FormatException();
            }

            byte a, r, g, b;

            Span<char> buffer = stackalloc char[2];
            var ns = NumberStyles.HexNumber;
            var ci = CultureInfo.InvariantCulture;

            var data = hex.AsSpan();
            if (hex[0] == '#')
            {
                data = data[1..];
            }

            // A
            switch (data.Length)
            {
                case 4:
                    // A
                    buffer[0] = data[0];
                    buffer[1] = data[0];
                    if (!byte.TryParse(buffer, ns, ci, out a))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    data = data[1..];
                    break;
                case 8:
                    // A
                    if (!byte.TryParse(data[..2], ns, ci, out a))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    data = data[2..];
                    break;
                default:
                    a = 0xFF;
                    break;
            }

            // RGB
            switch (data.Length)
            {
                case 3:
                    // R
                    buffer[0] = data[0];
                    buffer[1] = data[0];
                    if (!byte.TryParse(buffer, ns, ci, out r))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    // G
                    buffer[0] = data[1];
                    buffer[1] = data[1];
                    if (!byte.TryParse(buffer, ns, ci, out g))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    // B
                    buffer[0] = data[2];
                    buffer[1] = data[2];
                    if (!byte.TryParse(buffer, ns, ci, out b))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    break;
                case 6:
                    // R
                    if (!byte.TryParse(data[..2], ns, ci, out r))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    // G
                    if (!byte.TryParse(data[2..4], ns, ci, out g))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    // B
                    if (!byte.TryParse(data[4..6], ns, ci, out b))
                    {
                        return defaultColor ?? throw new FormatException();
                    }
                    break;
                default:
                    return defaultColor ?? throw new FormatException();
            }

            return Color.FromArgb(withAlpha ? a : (byte)0xFF, r, g, b);
        }

        public static string ToHex(this Color color, bool withAlpha = false)
        {
            var sb = new StringBuilder(withAlpha ? 9 : 7);

            sb.Append('#');

            if (withAlpha)
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
