using Rekog.Controllers.Options;
using Rekog.Core;
using Rekog.Core.Ngram;
using Rekog.Core.Ngrams;
using Rekog.IO;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class NgramController : ControllerBase<NgramOptions>
    {
        public NgramController(NgramOptions options, IConsole console, IFileSystem fileSystem) : base(options, console, fileSystem)
        {
        }

        public override async Task HandleAsync()
        {
            var ngrams = Options.Analyzed ? await ReadAnalyzed() : await Analyze();
            await Write(ngrams);
        }

        private async Task<NgramCollection> Analyze()
        {
            var paths = PathHelper.GetPaths(FileSystem, Options.Input, Options.Pattern);
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
                    using (var dataReader = CreateDataReader(path))
                    {
                        await analyzer.AnalyzeNext(dataReader);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"File error: {path} => {e.Message}");
                }
            }

            return analyzer.CreateResult();
        }

        private async Task<Alphabet> ReadAlphabet()
        {
            if (Options.Alphabets == null)
            {
                return new Alphabet();
            }

            var characters = new List<char>();

            foreach (var path in Options.Alphabets)
            {
                using (var dataReader = CreateDataReader(path))
                using (var reader = new AlphabetReader(dataReader))
                {
                    characters.AddRange(await reader.ReadCharacters());
                }
            }

            return new Alphabet(characters, Options.CaseSensitive);
        }

        private async Task<NgramCollection> ReadAnalyzed()
        {
            using (var dataReader = CreateDataReader(Options.Input))
            using (var ngramsReader = new NgramCollectionReader(dataReader))
            {
                return await ngramsReader.Read();
            }
        }

        private async Task Write(NgramCollection ngrams)
        {
            using (var dataWriter = CreateDataWriter(Options.Output))
            using (var ngramsWriter = new NgramCollectionWriter(dataWriter, Options.Raw))
            {
                await ngramsWriter.Write(ngrams, Options.TopCount ?? int.MaxValue);
            }
        }
    }
}
