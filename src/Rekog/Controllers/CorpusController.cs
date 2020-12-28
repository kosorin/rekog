using Rekog.Core;
using Rekog.Core.Corpora;
using Rekog.Data;
using Rekog.Data.Serialization;
using Rekog.Extensions;
using Rekog.IO;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class CorpusController
    {
        private readonly Options _options;
        private readonly Config _config;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public CorpusController(Options options, Config config, IFileSystem fileSystem, ILogger logger)
        {
            _options = options;
            _config = config;
            _fileSystem = fileSystem;
            _logger = logger.ForContext<CorpusController>();
        }

        public CorpusData? GetCorpusData(CancellationToken cancellationToken)
        {
            var datafile = GetCorpusDataFile();
            if (!TryLoadCorpusData(datafile, out var data))
            {
                data = ParseCorpus(cancellationToken);

                if (data != null)
                {
                    SaveCorpusData(datafile, data);
                }
            }

            return data;
        }

        private DataFile GetCorpusDataFile()
        {
            return new DataFile(_fileSystem, "corpus", _options.Alphabet, _options.Corpus + ".yml");
        }

        private bool TryLoadCorpusData(DataFile datafile, [MaybeNullWhen(false)] out CorpusData data)
        {
            if (!datafile.TryGetReader(out var reader))
            {
                _logger.Debug("Corpus data {CorpusDataFile} not found", datafile.Path);

                data = null;
                return false;
            }

            using (reader)
            {
                var report = new CorpusReportSerializer().Deserialize(reader);
                _logger.Information("Loaded corpus data {CorpusDataFile}", datafile.Path);

                data = ReportToData(report);
                return true;
            }

            static CorpusData ReportToData(CorpusReport report)
            {
                return new CorpusData(new(report.Unigrams), new(report.Bigrams), new(report.Trigrams));
            }
        }

        private void SaveCorpusData(DataFile datafile, CorpusData data)
        {
            var report = DataToReport(data);
            using (var writer = datafile.GetWriter())
            {
                new CorpusReportSerializer().Serialize(writer, report);
            }

            _logger.Information("Saved corpus data {CorpusDataFile}", datafile.Path);

            static CorpusReport DataToReport(CorpusData data)
            {
                return new CorpusReport
                {
                    Unigrams = GetRawCollection(data.UnigramOccurrences),
                    Bigrams = GetRawCollection(data.BigramOccurrences),
                    Trigrams = GetRawCollection(data.TrigramOccurrences),
                };

                static Dictionary<string, ulong> GetRawCollection(OccurrenceCollection<string> ngramOccurrences)
                {
                    return ngramOccurrences.OrderByDescending(x => x.Count).ToDictionary(x => x.Value, x => x.Count);
                }
            }
        }

        private CorpusData? ParseCorpus(CancellationToken cancellationToken)
        {
            var data = new CorpusData();

            var files = GetCorpusFiles();
            if (files.Count == 0)
            {
                return data;
            }

            var alphabet = GetAlphabet();

            _logger.Debug("Started parsing {FileCount} corpus file(s)", files.Count);

            var sw = Stopwatch.StartNew();
            Parallel.ForEach(files, new ParallelOptions(),
                () => new CorpusParser(alphabet),
                (file, state, _, parser) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    ParseCorpusFile(file, parser, cancellationToken);

                    return parser;
                },
                parser =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    lock (data)
                    {
                        data.Add(parser.GetData());
                    }
                });
            sw.Stop();

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            _logger.Information("Parsed {FileCount} corpus file(s) in {ElapsedTime} ({MillisecondsPerFile} ms/file)",
                files.Count,
                sw.Elapsed,
                files.Count > 0 ? (int)(sw.Elapsed.TotalMilliseconds / files.Count) : 0);
            return data;
        }

        private void ParseCorpusFile(CorpusFile file, CorpusParser parser, CancellationToken cancellationToken)
        {
            using (var reader = file.Open(_fileSystem))
            {
                parser.Parse(reader, cancellationToken);
            }

            _logger.Information("Parsed corpus file {CorpusFile}", file.Path);
        }

        private Alphabet GetAlphabet()
        {
            var alphabetConfig = _config.Alphabets[_options.Alphabet];
            var characters = alphabetConfig.Characters;
            if (!alphabetConfig.IncludeWhitespace)
            {
                characters = Regex.Replace(characters, @"\s+", "");
            }
            return new Alphabet(characters);
        }

        private List<CorpusFile> GetCorpusFiles()
        {
            var corpusConfig = _config.Corpora[_options.Corpus];

            if (corpusConfig.Path == null)
            {
                return new List<CorpusFile>();
            }

            return PathHelper
                .GetPaths(_fileSystem, corpusConfig.Path, corpusConfig.Pattern, corpusConfig.Recursive)
                .DistinctBy(x => x)
                .Select(path => new CorpusFile(path, string.IsNullOrWhiteSpace(corpusConfig.Encoding)
                    ? CorpusFile.DefaultEncoding
                    : Encoding.GetEncoding(corpusConfig.Encoding)))
                .ToList();
        }
    }
}
