using System.Windows.Media;

namespace Rekog.App.Extensions
{
    public static class StringExtensions
    {
        public static Color ToColor(this string value)
        {
            return (Color)ColorConverter.ConvertFromString(value);
        }
    }
}
