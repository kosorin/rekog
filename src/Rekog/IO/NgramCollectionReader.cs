using Rekog.Core.Ngrams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class NgramCollectionReader : DataReader
    {
        public NgramCollectionReader(string path) : base(path)
        {
        }

        public async Task<NgramCollection> Read()
        {
            var rawNgrams = new List<RawNgram>();

            while (true)
            {
                var line = await Reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                var parts = line.Split(FileFormat.Delimiter);
                if (parts.Length < 2)
                {
                    throw new FormatException();
                }

                var value = parts[0];
                var occurrences = ulong.TryParse(parts[1], out var number) ? number : throw new FormatException();

                rawNgrams.Add(new RawNgram
                {
                    Value = value,
                    Occurrences = occurrences
                });
            }

            return new NgramCollection(rawNgrams);
        }
    }
}
