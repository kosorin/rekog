using Rekog.Core;
using Rekog.Persistence;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.Commands.Ngram
{
    public class NgramLayoutCommand : CommandBase
    {
        public static Command Create()
        {
            var command = new Command("layout")
            {
                new Option<int>(new[] { "--size", "-n" }, () => 2),
                new Option<FileInfo>(new[] { "--ngrams", "-g" }),
                new Option<FileInfo>(new[] { "--layout", "-l" }),
                new Option<FileInfo?>(new[] { "--output", "-o" }),
            };
            command.Handler = HandlerDescriptor.FromDelegate((Func<int, FileInfo, FileInfo, FileInfo?, Task>)HandleStatic).GetCommandHandler();
            return command;

            static async Task HandleStatic(int size, FileInfo ngrams, FileInfo layout, FileInfo? output)
            {
                await new NgramLayoutCommand(size, ngrams, layout, output).Handle();
            }
        }

        private readonly int _size;
        private readonly FileInfo _ngrams;
        private readonly FileInfo _layout;
        private readonly FileInfo? _output;

        public NgramLayoutCommand(int size, FileInfo ngrams, FileInfo layout, FileInfo? output)
        {
            _size = size;
            _ngrams = ngrams;
            _layout = layout;
            _output = output;
        }

        public override async Task Handle()
        {
            using var layoutStreamReader = new StreamReader(_layout.FullName);
            using var layoutReader = new LayoutReader(layoutStreamReader);
            var layout = await layoutReader.Read();

            using var statisticsStreamReader = new StreamReader(_ngrams.FullName);
            using var statisticsReader = new NgramStatisticsReader(statisticsStreamReader);
            var statistics = await statisticsReader.Read(_size);

            var layoutNgrams = new Dictionary<Finger, List<NgramResult>>();
            foreach ((var finger, var characters) in layout)
            {
                layoutNgrams[finger] = statistics.SortedList
                    .Where(ngram => ngram.All(x => characters.Any(c => char.ToUpper(c) == char.ToUpper(x))))
                    .Select(x => new NgramResult(x, statistics.Ranks[x], statistics.Percentages[x]))
                    .Where(x => x.Percentages >= 0.001m)
                    .OrderBy(x => x.Rank)
                    .ToList();
            }

            await Write(layout, layoutNgrams);
        }

        private async Task Write(Dictionary<Finger, HashSet<char>> layout, Dictionary<Finger, List<NgramResult>> layoutNgrams)
        {
            var textWriter = _output != null
                ? new StreamWriter(_output.FullName)
                : Console.Out;
            try
            {
                foreach ((var finger, var ngramResults) in layoutNgrams)
                {
                    await textWriter.WriteLineAsync($"> {finger} ({string.Join(", ", layout[finger])})");
                    foreach (var ngram in ngramResults)
                    {
                        await textWriter.WriteLineAsync($"    {ngram.Rank,3}. {ngram.Ngram} ({ngram.Percentages:P3})");
                    }
                }
            }
            finally
            {
                if (_output != null)
                {
                    textWriter?.Dispose();
                }
            }
        }

        private class NgramResult
        {
            public NgramResult(string ngram, int rank, decimal percentages)
            {
                Ngram = ngram;
                Rank = rank;
                Percentages = percentages;
            }

            public string Ngram { get; }

            public int Rank { get; }

            public decimal Percentages { get; }
        }
    }
}
