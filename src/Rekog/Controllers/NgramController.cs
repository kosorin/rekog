using Rekog.Controllers.Options;
using Rekog.Core;
using Rekog.Core.Ngram;
using Rekog.Core.Ngrams;
using Rekog.IO;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class NgramController : ControllerBase<NgramOptions>
    {
        public NgramController(NgramOptions options, InvocationContext context) : base(options, context)
        {
        }

        public override async Task HandleAsync()
        {
            var ngrams = Options.Analyzed ? await ReadAnalyzed() : await Analyze();
            await Write(ngrams);
        }

        private async Task<NgramCollection> Analyze()
        {
            var paths = GetInputPaths();
            if (!paths.Any())
            {
                return new NgramCollection();
            }

            var alphabet = await ReadAlphabet();

            var analyzer = new NgramAnalyzer(Options.Size, Options.CaseSensitive, alphabet);

            foreach (var path in paths)
            {
                try
                {
                    Console.Out.WriteLine($"File: {path}");
                    using (var reader = new StreamReader(path.FullName))
                    {
                        await analyzer.AnalyzeNext(reader);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"File error: {path} => {e.Message}");
                }
            }

            return analyzer.CreateResult();
        }

        private List<FileInfo> GetInputPaths()
        {
            var attributes = File.GetAttributes(Options.Input);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                var pattern = string.IsNullOrWhiteSpace(Options.Pattern) ? "*" : Options.Pattern;
                var allDirectories = false;
                if (pattern[0] == '/')
                {
                    pattern = pattern[1..];
                    allDirectories = true;
                }
                return Directory.GetFiles(Options.Input, pattern, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)).ToList();
            }
            else
            {
                return new List<FileInfo> { new FileInfo(Options.Input) };
            }
        }

        private async Task<Alphabet> ReadAlphabet()
        {
            if (Options.Alphabets == null)
            {
                return new Alphabet();
            }

            var characters = new List<char>();

            foreach (var fileInfo in Options.Alphabets)
            {
                using (var reader = new AlphabetReader(fileInfo.FullName))
                {
                    characters.AddRange(await reader.ReadCharacters());
                }
            }

            return new Alphabet(characters, Options.CaseSensitive);
        }

        private async Task<NgramCollection> ReadAnalyzed()
        {
            using (var ngramsReader = new NgramCollectionReader(Options.Input))
            {
                return await ngramsReader.Read();
            }
        }

        private async Task Write(NgramCollection ngrams)
        {
            using (var outputWriter = CreateOutputWriter(Options.Output))
            using (var ngramsWriter = new NgramCollectionWriter(outputWriter, Options.Raw))
            {
                await ngramsWriter.Write(ngrams, Options.TopCount ?? int.MaxValue);
            }
        }
    }
}
