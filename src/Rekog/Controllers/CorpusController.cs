using Rekog.Core.Corpora;
using Rekog.Extensions;
using Rekog.IO;
using Rekog.Persistence;
using Rekog.Persistence.Serialization;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class CorpusController : CommandController<CorpusCommandConfig, CorpusCommandOptions>
    {
        public CorpusController(CorpusCommandConfig config, IConsole console, IFileSystem fileSystem) : base(config, console, fileSystem)
        {
        }

        public override Task<int> HandleAsync(CancellationToken cancellationToken)
        {
            var alphabet = GetAlphabet();
            var files = GetFiles();

            var report = AnalyzeFiles(alphabet, files, cancellationToken);
            if (report != null)
            {
                SaveReport(report);
                return Task.FromResult(0);
            }
            else
            {
                return Task.FromResult(1);
            }
        }

        private Alphabet GetAlphabet()
        {
            IEnumerable<AlphabetConfig> alphabetConfigs;
            if (Config.Options.Alphabet.Length == 0)
            {
                if (Config.Alphabets.Count == 1)
                {
                    alphabetConfigs = Config.Alphabets.Values;
                }
                else
                {
                    alphabetConfigs = Enumerable.Empty<AlphabetConfig>();
                }
            }
            else
            {
                alphabetConfigs = from o in Config.Options.Alphabet
                                  join c in Config.Alphabets on o equals c.Key
                                  select c.Value;
            }
            return new Alphabet(alphabetConfigs.SelectMany(x => x.IncludeWhitespace ? x.Characters : Regex.Replace(x.Characters, @"\s+", "")));
        }

        private List<CorpusFile> GetFiles()
        {
            IEnumerable<LocationConfig> locationConfigs;
            if (Config.Options.Corpus.Length == 0)
            {
                if (Config.Locations.Count == 1)
                {
                    locationConfigs = Config.Locations.Values;
                }
                else
                {
                    locationConfigs = Enumerable.Empty<LocationConfig>();
                }
            }
            else
            {
                locationConfigs = from o in Config.Options.Corpus
                                  join c in Config.Locations on o equals c.Key
                                  select c.Value;
            }
            return locationConfigs
                .SelectMany(x => PathHelper
                    .GetPaths(FileSystem, x.Path, x.Pattern, x.Recursive)
                    .Select(p => new CorpusFile(p, Encoding.GetEncoding(x.Encoding))))
                .DistinctBy(x => x.Path)
                .ToList();
        }

        private CorpusReport? AnalyzeFiles(Alphabet alphabet, List<CorpusFile> files, CancellationToken cancellationToken)
        {
            var mainAnalyzer = BuildAnalyzer();

            Console.Out.WriteLine($"Started: {DateTime.Now}");
            var sw = Stopwatch.StartNew();

            var parallelLoopResult = Parallel.ForEach(files, new ParallelOptions(),
                BuildAnalyzer,
                (file, state, i, analyzer) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    Console.Out.WriteLine($"File: {file.Path}");
                    using var reader = file.Open(FileSystem);
                    analyzer.Analyze(reader, cancellationToken);
                    return analyzer;
                },
                analyzer =>
                {
                    lock (mainAnalyzer)
                    {
                        mainAnalyzer.Append(analyzer);
                    }
                });

            sw.Stop();
            Console.Out.WriteLine($"{(parallelLoopResult.IsCompleted ? "Finished" : "Interrupted")}: {DateTime.Now}");
            Console.Out.WriteLine($"Files: {files.Count}");
            Console.Out.WriteLine($"Elapsed time: {sw.Elapsed} ({(files.Count > 0 ? (int)(sw.Elapsed.TotalMilliseconds / files.Count) : 0):N0} ms/file)");

            return parallelLoopResult.IsCompleted ? mainAnalyzer.CreateReport() : null;

            CorpusAnalyzer BuildAnalyzer() => new CorpusAnalyzer(alphabet, Config.Options.CaseSensitive, Config.Options.Extended);
        }

        private void SaveReport(CorpusReport report)
        {
            using var stream = FileSystem.FileStream.Create(Config.Options.Output, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            new CorpusReportSerializer().Serialize(writer, report);
        }
    }
}
