using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Rekog.Core;
using Rekog.Core.Corpora;
using Rekog.Data;
using Rekog.Data.Serialization;
using Rekog.Extensions;
using Rekog.IO;
using Serilog;

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

        public CorpusAnalysisData? GetCorpusAnalysisData(CancellationToken cancellationToken)
        {
            var analysisDataFile = GetCorpusDataFile();
            if (!TryLoadAnalysisData(analysisDataFile, out var analysisData))
            {
                analysisData = AnalyzeCorpus(cancellationToken);
                if (analysisData != null)
                {
                    SaveAnalysisData(analysisDataFile, analysisData);
                }
            }

            return analysisData;
        }

        private DataFile GetCorpusDataFile()
        {
            return new DataFile(_fileSystem, "output", "corpora", _options.Corpus, _options.Alphabet + ".yml");
        }

        private bool TryLoadAnalysisData(DataFile analysisDatafile, [MaybeNullWhen(false)] out CorpusAnalysisData analysisData)
        {
            if (!analysisDatafile.TryGetReader(out var reader))
            {
                _logger.Debug("Corpus analysis data {CorpusAnalysisDataFile} not found", analysisDatafile.Path);

                analysisData = null;
                return false;
            }

            using (reader)
            {
                var report = new CorpusAnalysisReportSerializer().Deserialize(reader);
                _logger.Information("Loaded corpus analysis data {CorpusAnalysisDataFile}", analysisDatafile.Path);

                analysisData = ReportToData(report);
                return true;
            }

            static CorpusAnalysisData ReportToData(CorpusAnalysisReport report)
            {
                return new CorpusAnalysisData(new OccurrenceCollection<string>(report.Unigrams.ToDictionary(x => x.Key.Value, x => x.Value)),
                    new OccurrenceCollection<string>(report.Bigrams.ToDictionary(x => x.Key.Value, x => x.Value)),
                    new OccurrenceCollection<string>(report.Trigrams.ToDictionary(x => x.Key.Value, x => x.Value)),
                    new OccurrenceCollection<Rune>(report.Replaced.ToDictionary(x => x.Key.Value.EnumerateRunes().First(), x => x.Value)),
                    new OccurrenceCollection<Rune>(report.Skipped.ToDictionary(x => x.Value.Value.EnumerateRunes().First(), x => x.Count)));
            }
        }

        private void SaveAnalysisData(DataFile analysisDatafile, CorpusAnalysisData analysisData)
        {
            var report = DataToReport(analysisData);
            using (var writer = analysisDatafile.GetWriter())
            {
                new CorpusAnalysisReportSerializer().Serialize(writer, report);
            }

            _logger.Information("Saved corpus analysis data {CorpusAnalysisDataFile}", analysisDatafile.Path);

            static CorpusAnalysisReport DataToReport(CorpusAnalysisData data)
            {
                return new CorpusAnalysisReport
                {
                    Unigrams = GetRawCollection(data.Unigrams),
                    Bigrams = GetRawCollection(data.Bigrams),
                    Trigrams = GetRawCollection(data.Trigrams),
                    Replaced = data.ReplacedCharacters.OrderByDescending(x => x.Count).ToDictionary(x => new ReportToken(x.Value.ToString()), x => x.Count),
                    Skipped = data.SkippedCharacters.Select(x => new SkippedToken(x.Count, x.Value.ToString(), x.Value.Value))
                        .OrderByDescending(x => x.Count)
                        .ToList(),
                };

                static Dictionary<ReportToken, ulong> GetRawCollection(OccurrenceCollection<string> ngramOccurrences)
                {
                    return ngramOccurrences.OrderByDescending(x => x.Count).ToDictionary(x => new ReportToken(x.Value), x => x.Count);
                }
            }
        }

        private CorpusAnalysisData? AnalyzeCorpus(CancellationToken cancellationToken)
        {
            var fileInfos = GetCorpusFileInfos();
            if (fileInfos.Count == 0)
            {
                return new CorpusAnalysisData();
            }

            var alphabet = GetAlphabet();

            _logger.Debug("Started analyzing {FileCount} corpus file(s)", fileInfos.Count);
            var sw = Stopwatch.StartNew();

            var analysisData = AnalyzeCorpusCore(fileInfos, alphabet, cancellationToken);
            if (analysisData == null)
            {
                return null;
            }

            sw.Stop();
            _logger.Information("Analyzed {FileCount} corpus file(s) in {ElapsedTime} ({MillisecondsPerFile} ms/file)",
                fileInfos.Count,
                sw.Elapsed,
                fileInfos.Count > 0 ? (int)(sw.Elapsed.TotalMilliseconds / fileInfos.Count) : 0);

            return analysisData;
        }

        private CorpusAnalysisData? AnalyzeCorpusCore(List<CorpusFileInfo> fileInfos, Alphabet alphabet, CancellationToken cancellationToken)
        {
            var analysisData = new CorpusAnalysisData();

            var parallelLoopResult = Parallel.ForEach(fileInfos, new ParallelOptions(),
                () => new CorpusAnalyzer(alphabet, _logger.ForContext<CorpusAnalyzer>()),
                (fileInfo, state, _, analyzer) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }
                    else
                    {
                        AnalyzeCorpusFile(fileInfo, analyzer, cancellationToken);
                    }

                    return analyzer;
                },
                analyzer =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    lock (analysisData)
                    {
                        analysisData.Add(analyzer.GetAnalysisData());
                    }
                });

            return parallelLoopResult.IsCompleted && !cancellationToken.IsCancellationRequested
                ? analysisData
                : null;
        }

        private void AnalyzeCorpusFile(CorpusFileInfo fileInfo, CorpusAnalyzer analyzer, CancellationToken cancellationToken)
        {
            using (var reader = fileInfo.Open(_fileSystem))
            {
                analyzer.AnalyzeNext(reader, cancellationToken);
            }

            _logger.Information("Analyzed corpus file {CorpusFile}", fileInfo.Path);
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

        private List<CorpusFileInfo> GetCorpusFileInfos()
        {
            var corpusConfig = _config.Corpora[_options.Corpus];

            if (corpusConfig.Path == null)
            {
                return new List<CorpusFileInfo>();
            }

            return PathHelper
                .GetPaths(_fileSystem, corpusConfig.Path, corpusConfig.Pattern, corpusConfig.Recursive)
                .DistinctBy(x => x)
                .Select(path => new CorpusFileInfo(path, string.IsNullOrWhiteSpace(corpusConfig.Encoding)
                    ? CorpusFileInfo.DefaultEncoding
                    : Encoding.GetEncoding(corpusConfig.Encoding)))
                .ToList();
        }
    }
}
