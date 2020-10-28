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
    public class NgramAnalyzeCommand : CommandBase
    {
        public static Command Create()
        {
            var command = new Command("analyze")
            {
                new Option<int>(new[] { "--size", "-n" }, () => 2),
                new Option<bool>(new[] { "--case-sensitive", "-c" }),
                new Option<string?>(new[] { "--alphabet", "-a" }),
                new Option<int?>(new[] { "--top-count", "-t" }),
                new Option<string?>(new[] { "--pattern", "-p" }),
                new Option<DirectoryInfo?>(new[] { "--input-directory", "-d" }),
                new Option<FileInfo?>(new[] { "--input", "-i" }),
                new Option<FileInfo?>(new[] { "--output", "-o" }),
            };
            command.Handler = HandlerDescriptor.FromDelegate((Func<int, bool, string?, int?, string?, DirectoryInfo?, FileInfo?, FileInfo?, Task>)HandleStatic).GetCommandHandler();
            return command;

            static async Task HandleStatic(int size, bool caseSensitive, string? alphabet, int? topCount, string? pattern, DirectoryInfo? inputDirectory, FileInfo? input, FileInfo? output)
            {
                await new NgramAnalyzeCommand(size, caseSensitive, alphabet, topCount, pattern, inputDirectory, input, output).Handle();
            }
        }

        private readonly int _size;
        private readonly bool _caseSensitive;
        private readonly string? _alphabet;
        private readonly int? _topCount;
        private readonly string? _pattern;
        private readonly DirectoryInfo? _inputDirectory;
        private readonly FileInfo? _input;
        private readonly FileInfo? _output;

        public NgramAnalyzeCommand(int size, bool caseSensitive, string? alphabet, int? topCount, string? pattern, DirectoryInfo? inputDirectory, FileInfo? input, FileInfo? output)
        {
            _size = size;
            _caseSensitive = caseSensitive;
            _alphabet = alphabet;
            _topCount = topCount;
            _pattern = pattern;
            _inputDirectory = inputDirectory;
            _input = input;
            _output = output;
        }

        public override async Task Handle()
        {
            await Write(await Analyze());
        }

        private async Task<NgramStatistics> Analyze()
        {
            var analyzer = new NgramAnalyzer(_size, _caseSensitive, _alphabet);

            foreach (var path in GetInputPaths())
            {
                try
                {
                    Console.WriteLine($"File: {path}");
                    using (var reader = new StreamReader(path.FullName))
                    {
                        await analyzer.Analyze(reader);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"File error: {e.Message}");
                }
            }

            return analyzer.GetStatistics();
        }

        private List<FileInfo> GetInputPaths()
        {
            var paths = new List<FileInfo>();

            if (_input != null)
            {
                paths.Add(_input);
            }

            if (_inputDirectory != null && !string.IsNullOrWhiteSpace(_pattern))
            {
                paths.AddRange(Directory.GetFiles(_inputDirectory.FullName, _pattern).Select(x => new FileInfo(x)));
            }

            return paths;
        }

        private async Task Write(NgramStatistics statistics)
        {
            var textWriter = _output != null
                ? new StreamWriter(_output.FullName)
                : Console.Out;
            try
            {
                using (var writer = new NgramStatisticsWriter(textWriter))
                {
                    await writer.Write(statistics, _topCount);
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
    }
}
