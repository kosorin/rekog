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
                    .GetPaths(FileSystem, x.Path, x.SearchPattern, x.Recursive)
                    .Select(p => new CorpusFile(p, Encoding.GetEncoding(x.Encoding))))
                .DistinctBy(x => x.Path)
                .ToList();
        }

        private CorpusReport AnalyzeFiles(Alphabet alphabet, List<CorpusFile> files, CancellationToken cancellationToken)
        {
            var interrupted = false;

            Console.Out.WriteLine($"Started: {DateTime.Now}");
            var sw = Stopwatch.StartNew();

            var i = 0;
            var analyzer = new CorpusAnalyzer(alphabet, Config.Options.CaseSensitive, Config.Options.IncludeIgnored);
            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    interrupted = true;
                    break;
                }

                i++;
                Console.Out.WriteLine($"File [{new string(' ', (int)Math.Log10(files.Count) - (int)Math.Log10(i))}{i}/{files.Count}]: {file.Path}");
                using var reader = file.Open(FileSystem);
                analyzer.Analyze(reader, cancellationToken);
            }
            var report = analyzer.CreateReport();

            sw.Stop();
            Console.Out.WriteLine($"{(interrupted ? "Interrupted" : "Finished")}: {DateTime.Now}");
            Console.Out.WriteLine($"Elapsed time: {sw.Elapsed}");

            return report;
        }

        private void SaveReport(CorpusReport report)
        {
            using var stream = FileSystem.FileStream.Create(Config.Options.Output, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            new CorpusReportSerializer().Serialize(writer, report);
        }
    }
}
