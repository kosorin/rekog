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

        public override int Handle(CancellationToken cancellationToken)
        {
            var alphabet = GetAlphabet();
            var files = GetFiles();

            var report = ParseFiles(files, alphabet, cancellationToken);
            if (report != null)
            {
                SaveReport(report);
                return 0;
            }
            else
            {
                return 1;
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
                .SelectMany(config => PathHelper
                    .GetPaths(FileSystem, config.Path, config.Pattern, config.Recursive)
                    .Select(path => new CorpusFile(path, string.IsNullOrWhiteSpace(config.Encoding)
                        ? Encoding.UTF8
                        : Encoding.GetEncoding(config.Encoding))))
                .DistinctBy(x => x.Path)
                .ToList();
        }

        private CorpusReport? ParseFiles(List<CorpusFile> files, Alphabet alphabet, CancellationToken cancellationToken)
        {
            var data = new CorpusData();

            Console.Out.WriteLine($"Started: {DateTime.Now}");
            var sw = Stopwatch.StartNew();

            var parallelLoopResult = Parallel.ForEach(files, new ParallelOptions(),
                () => new CorpusParser(alphabet, Config.Options.CaseSensitive),
                (file, state, i, parser) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    // Use \n instead of WriteLine because WriteLine extension is not thread safe and produce bad output
                    Console.Out.Write($"File: {file.Path}\n");
                    using (var reader = file.Open(FileSystem))
                    {
                        parser.Parse(reader, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    return parser;
                },
                parser =>
                {
                    lock (data)
                    {
                        data.Add(parser.GetData());
                    }
                });

            sw.Stop();
            if (parallelLoopResult.IsCompleted)
            {
                Console.Out.WriteLine($"Finished: {DateTime.Now}");
                Console.Out.WriteLine($"Files: {files.Count}");
                Console.Out.WriteLine($"Elapsed time: {sw.Elapsed} ({(files.Count > 0 ? (int)(sw.Elapsed.TotalMilliseconds / files.Count) : 0):N0} ms/file)");
            }
            else
            {
                Console.Out.WriteLine($"Interrupted: {DateTime.Now}");
            }

            return parallelLoopResult.IsCompleted ? data.ToReport() : null;
        }

        private void SaveReport(CorpusReport report)
        {
            using var stream = FileSystem.FileStream.Create(Config.Options.Output, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            new CorpusReportSerializer().Serialize(writer, report);
        }
    }
}
