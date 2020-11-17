using Rekog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class LayoutReader : ReaderBase
    {
        public LayoutReader(IDataReader reader) : base(reader)
        {
        }

        public async Task<Dictionary<Finger, HashSet<char>>> Read()
        {
            var headerLine = await Reader.ReadLineAsync();
            if (headerLine == null)
            {
                throw new FormatException();
            }

            var positions = headerLine.Split(FileFormat.Delimiter)
                .Select((x, i) => int.TryParse(x, out var n) ? new { Index = i, Finger = (Finger)n } : null)
                .Where(x => x != null)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                .ToDictionary(x => x.Index, x => x.Finger);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var rawData = Enum.GetValues(typeof(Finger)).Cast<Finger>().ToDictionary(x => x, _ => new HashSet<char>());

            while (true)
            {
                var line = await Reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                var characters = line.Split(FileFormat.Delimiter);
                foreach ((var index, var finger) in positions)
                {
                    if (index >= characters.Length)
                    {
                        continue;
                    }
                    foreach (var character in characters[index])
                    {
                        rawData[finger].Add(character);
                    }
                }
            }

            return rawData;
        }
    }
}
