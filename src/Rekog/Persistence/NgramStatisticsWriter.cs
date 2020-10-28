using Rekog.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.Persistence
{
    public class NgramStatisticsWriter : PersistenceWriter
    {
        public NgramStatisticsWriter(TextWriter writer) : base(writer)
        {
        }

        public Task Write(NgramStatistics statistics)
        {
            return Write(statistics, null);
        }

        public async Task Write(NgramStatistics statistics, int? topCount)
        {
            foreach (var ngram in statistics.SortedList.Take(topCount ?? int.MaxValue))
            {
                await Writer.WriteLineAsync($"{ngram}\t{statistics.RawData[ngram]}");
            }
        }
    }
}
