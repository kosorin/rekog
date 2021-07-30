using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rekog.Core.Layouts
{
    public class LayoutAnalysisNode
    {
        public LayoutAnalysisNode(string description, double? percentage, double? effort)
        {
            Description = description;
            Percentage = percentage;
            Effort = effort;
            Children = new List<LayoutAnalysisNode>();
        }

        public LayoutAnalysisNode(string description, List<LayoutAnalysisNode> children)
        {
            Description = description;
            Percentage = GetTotalPercentage(children);
            Effort = GetTotalEffort(children);
            Children = children;
        }

        public string Description { get; }

        public List<LayoutAnalysisNode> Children { get; }

        public double? Percentage { get; }

        public double? Effort { get; }

        public void Print()
        {
            Print(0);
        }

        private void Print(int indent)
        {
            const int maxDescriptionLength = 40;

            var sb = new StringBuilder(new string(' ', indent));
            var description = string.Concat(Description.Take(maxDescriptionLength - indent));
            sb.Append(description);
            sb.Append(new string(' ', maxDescriptionLength - indent - description.Length));
            sb.Append($"  {(indent > 0 ? Percentage : null),10:P3}");
            sb.Append($"  {Effort,10:N5}");
            Console.WriteLine(sb);

            var children = Children
                .OrderByDescending(x => x.Effort)
                .ThenByDescending(x => x.Percentage)
                .ThenByDescending(x => x.Description);
            foreach (var child in children)
            {
                child.Print(indent + 2);
            }
        }

        private static double? GetTotalPercentage(List<LayoutAnalysisNode> nodes)
        {
            return nodes.Count > 0
                ? nodes.Select(x => x.Percentage).Aggregate((x, a) => a.HasValue ? a + (x ?? 0) : x)
                : null;
        }

        private static double? GetTotalEffort(List<LayoutAnalysisNode> nodes)
        {
            return nodes.Count > 0
                ? nodes.Select(x => x.Effort).Aggregate((x, a) => a.HasValue ? a + (x ?? 0) : x)
                : null;
        }
    }
}
