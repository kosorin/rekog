using System.IO;

namespace Rekog.Extensions
{
    public static class StreamReaderExtensions
    {
        public static bool TryReadChar(this StreamReader reader, out char character)
        {
            if (reader.EndOfStream)
            {
                character = default;
                return false;
            }

            var value = reader.Read();
            if (value == -1)
            {
                character = default;
                return false;
            }

            character = (char)value;
            return true;
        }
    }
}
