using Rekog.Core.Corpora;
using Rekog.Extensions;
using Rekog.IO;
using Rekog.Data;
using Rekog.Data.Serialization;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rekog
{
    public class Controller
    {
        private readonly Config _config;
        private readonly IConsole _console;
        private readonly IFileSystem _fileSystem;
        private readonly DataFileManager _dataFileManager;

        public Controller(Config config, DataFileManager dataFileManager, IConsole console, IFileSystem fileSystem)
        {
            _config = config;
            _dataFileManager = dataFileManager;
            _console = console;
            _fileSystem = fileSystem;
        }

        public void Handle(CancellationToken cancellationToken)
        {
            var corpusFiles = GetCorpusFiles();
            var alphabet = GetAlphabet();
            var corpusData = ParseCorpusFiles(corpusFiles, alphabet, cancellationToken);
            if (corpusData != null)
            {
                SaveCorpusReport(corpusData.ToReport());
            }
        }

        private Alphabet GetAlphabet()
        {
            var alphabetConfig = _config.Alphabets[_config.Options.Alphabet];
            var characters = alphabetConfig.Characters;
            if (!alphabetConfig.IncludeWhitespace)
            {
                characters = Regex.Replace(characters, @"\s+", "");
            }
            return new Alphabet(characters);
        }

        private List<CorpusFile> GetCorpusFiles()
        {
            var corpusConfig = _config.Corpora[_config.Options.Corpus];
            return PathHelper
                .GetPaths(_fileSystem, corpusConfig.Path, corpusConfig.Pattern, corpusConfig.Recursive)
                .DistinctBy(x => x)
                .Select(path => new CorpusFile(path, string.IsNullOrWhiteSpace(corpusConfig.Encoding)
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(corpusConfig.Encoding)))
                .ToList();
        }

        private CorpusData? ParseCorpusFiles(List<CorpusFile> corpusFiles, Alphabet alphabet, CancellationToken cancellationToken)
        {
            var data = new CorpusData();
            _console.Out.WriteLine($"Started: {DateTime.Now}");
            var sw = Stopwatch.StartNew();

            Parallel.ForEach(corpusFiles, new ParallelOptions(),
                () => new CorpusParser(alphabet, caseSensitive: false),
                (file, state, i, parser) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    // Use \n instead of WriteLine because WriteLine extension is not thread safe and produce bad output
                    _console.Out.Write($"File: {file.Path}\n");
                    using (var reader = file.Open(_fileSystem))
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
                cancellationToken.ThrowIfCancellationRequested();
                _console.Out.WriteLine($"Interrupted: {DateTime.Now}");
                return null;
            }
            else
            {
                _console.Out.WriteLine($"Finished: {DateTime.Now}");
                _console.Out.WriteLine($"Files: {corpusFiles.Count}");
                _console.Out.WriteLine($"Elapsed time: {sw.Elapsed} ({(corpusFiles.Count > 0 ? (int)(sw.Elapsed.TotalMilliseconds / corpusFiles.Count) : 0):N0} ms/file)");
                return data;
            }
        }

        private void SaveCorpusReport(CorpusReport report)
        {
            var writer = _dataFileManager.GetCorpusReportWriter(_config.Options.Corpus, _config.Options.Alphabet);
            new CorpusReportSerializer().Serialize(writer, report);
        }
    }
}
