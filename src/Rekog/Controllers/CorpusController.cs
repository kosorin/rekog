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
            var corpusFiles = GetCorpusFiles();

            var report = AnalyzeCorpusFiles(alphabet, corpusFiles);

            SaveReport(report);

            return Task.FromResult(0);
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

        private List<CorpusFile> GetCorpusFiles()
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
                    .GetPaths(FileSystem, x.Path, x.SearchPattern, x.Recursive)
                    .Select(p => new CorpusFile(p, Encoding.GetEncoding(x.Encoding))))
                .DistinctBy(x => x.Path)
                .ToList();
        }

        private CorpusReport AnalyzeCorpusFiles(Alphabet alphabet, List<CorpusFile> corpusFiles)
        {
            Console.Out.WriteLine($"Started: {DateTime.Now}");
            var sw = Stopwatch.StartNew();

            var analyzer = new CorpusAnalyzer(alphabet, Config.Options.CaseSensitive, Config.Options.IncludeIgnored);
            foreach (var corpusFile in corpusFiles)
            {
                Console.Out.WriteLine($"File: {corpusFile.Path}");
                analyzer.Analyze(FileSystem, corpusFile);
            }
            var report = analyzer.CreateReport();

            sw.Stop();
            Console.Out.WriteLine($"Finished: {DateTime.Now}");
            Console.Out.WriteLine($"Elapsed time: {sw.Elapsed}");

            return report;
        }

        private void SaveReport(CorpusReport report)
        {
            using (var outputStream = FileSystem.FileStream.Create(Config.Options.Output, FileMode.Create, FileAccess.Write))
            using (var outputWriter = new StreamWriter(outputStream, Encoding.UTF8))
            {
                new CorpusReportSerializer().Serialize(outputWriter, report);
            }
        }
    }
}
