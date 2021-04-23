using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts
{
    public class LayoutAnalysisResult
    {
        public LayoutAnalysisResult(string description)
            : this(description, new List<LayoutAnalysisResult>())
        {
        }

        public LayoutAnalysisResult(string description, List<LayoutAnalysisResult> items)
        {
            Description = description;
            Items = items;
        }

        public string Description { get; }

        public List<LayoutAnalysisResult> Items { get; }

        public double? Percentage { get; init; }

        public double? Effort { get; init; }

        public double? GetTotalPercentage()
        {
            var itemsPercentage = Items.Count > 0 ? Items.Select(x => x.GetTotalPercentage()).Aggregate((x, a) => a.HasValue ? a + (x ?? 0) : x) : null;
            if (!itemsPercentage.HasValue && !Percentage.HasValue)
            {
                return null;
            }
            return (Percentage ?? 0) + (itemsPercentage ?? 0);
        }

        public double? GetTotalEffort()
        {
            var itemsEffort = Items.Count > 0 ? Items.Select(x => x.GetTotalEffort()).Aggregate((x, a) => a.HasValue ? a + (x ?? 0) : x) : null;
            if (!itemsEffort.HasValue && !Effort.HasValue)
            {
                return null;
            }
            return (Effort ?? 0) + (itemsEffort ?? 0);
        }
    }
}
