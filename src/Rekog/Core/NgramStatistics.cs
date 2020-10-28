using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core
{
    public class NgramStatistics
    {
        public Dictionary<string, decimal> RawData { get; }

        public decimal TotalCount { get; }

        public List<string> SortedList { get; }

        public Dictionary<string, int> Ranks { get; }

        public Dictionary<string, decimal> Percentages { get; }

        public NgramStatistics(Dictionary<string, decimal> rawData)
        {
            RawData = rawData;
            TotalCount = RawData.Sum(x => x.Value);
            SortedList = RawData.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            Ranks = SortedList.Select((x, i) => (Ngram: x, Rank: i + 1)).ToDictionary(x => x.Ngram, x => x.Rank);
            Percentages = RawData.ToDictionary(x => x.Key, x => x.Value / TotalCount);
        }
    }
}
