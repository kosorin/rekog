using System.Globalization;

namespace Rekog.IO
{
    public static class FileFormat
    {
        public static string Delimiter => "\t";

        public static string ItemDelimiter => " ";

        public static CultureInfo CultureInfo => CultureInfo.InvariantCulture;
    }
}
