using System;
using System.CommandLine.IO;

namespace Rekog.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteLine(this IStandardStreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(Environment.NewLine);
        }
        public static void WriteLine(this IStandardStreamWriter writer, string value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(value + Environment.NewLine);
        }
    }
}
