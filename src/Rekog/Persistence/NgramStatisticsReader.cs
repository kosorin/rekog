using Rekog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rekog.Persistence
{
    public class NgramStatisticsReader : PersistenceReader
    {
        public NgramStatisticsReader(StreamReader reader) : base(reader)
        {
        }

        public async Task<NgramStatistics> Read(int size)
        {
            var rawData = new Dictionary<string, decimal>();

            while (true)
            {
                var line = await Reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                var match = Regex.Match(line, @"^(?<Ngram>.+)\t(?<Count>\d+)$");
                if (!match.Success)
                {
                    throw new FormatException();
                }

                var ngram = match.Groups["Ngram"].Value;
                if (ngram.Length != size)
                {
                    throw new FormatException();
                }

                var count = decimal.Parse(match.Groups["Count"].Value);

                rawData.Add(ngram, count);
            }

            return new NgramStatistics(rawData);
        }
    }
}
