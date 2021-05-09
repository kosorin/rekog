using System;
using RgbColor = System.Windows.Media.Color;

namespace Rekog.App.Core
{
    public class HslColor
    {
        public HslColor(double h, double s, double l, double a)
        {
            H = h;
            S = s;
            L = l;
            A = a;
        }

        public HslColor(RgbColor rgb)
        {
            RgbToHls(rgb.R, rgb.G, rgb.B, out var h, out var l, out var s);
            H = h;
            L = l;
            S = s;
            A = rgb.A / 255d;
        }

        public double H { get; }

        public double S { get; }

        public double L { get; }

        public double A { get; }

        public RgbColor ToRgb()
        {
            int r, g, b;
            HlsToRgb(H, L, S, out r, out g, out b);
            return RgbColor.FromArgb((byte)(A * 255d), (byte)r, (byte)g, (byte)b);
        }

        public HslColor Lighten(double amount)
        {
            return new HslColor(H, S, Clamp(L * amount, 0, 1), A);
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }

            return value;
        }

        private static void RgbToHls(int r, int g, int b,
            out double h, out double l, out double s)
        {
            var doubleR = r / 255d;
            var doubleG = g / 255d;
            var doubleB = b / 255d;

            var max = doubleR;
            if (max < doubleG)
            {
                max = doubleG;
            }
            if (max < doubleB)
            {
                max = doubleB;
            }

            var min = doubleR;
            if (min > doubleG)
            {
                min = doubleG;
            }
            if (min > doubleB)
            {
                min = doubleB;
            }

            var diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;
            }
            else
            {
                if (l <= 0.5)
                {
                    s = diff / (max + min);
                }
                else
                {
                    s = diff / (2 - max - min);
                }

                var rDist = (max - doubleR) / diff;
                var gDist = (max - doubleG) / diff;
                var bDist = (max - doubleB) / diff;

                if (doubleR == max)
                {
                    h = bDist - gDist;
                }
                else if (doubleG == max)
                {
                    h = 2 + rDist - bDist;
                }
                else
                {
                    h = 4 + gDist - rDist;
                }

                h *= 60;
                if (h < 0)
                {
                    h += 360;
                }
            }
        }

        private static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5)
            {
                p2 = l * (1 + s);
            }
            else
            {
                p2 = l + s - l * s;
            }

            var p1 = 2 * l - p2;
            double doubleR, doubleG, doubleB;
            if (s == 0)
            {
                doubleR = l;
                doubleG = l;
                doubleB = l;
            }
            else
            {
                doubleR = QqhToRgb(p1, p2, h + 120);
                doubleG = QqhToRgb(p1, p2, h);
                doubleB = QqhToRgb(p1, p2, h - 120);
            }

            r = (int)(doubleR * 255d);
            g = (int)(doubleG * 255d);
            b = (int)(doubleB * 255d);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360)
            {
                hue -= 360;
            }
            else if (hue < 0)
            {
                hue += 360;
            }

            if (hue < 60)
            {
                return q1 + (q2 - q1) * hue / 60;
            }
            if (hue < 180)
            {
                return q2;
            }
            if (hue < 240)
            {
                return q1 + (q2 - q1) * (240 - hue) / 60;
            }
            return q1;
        }
    }
}
