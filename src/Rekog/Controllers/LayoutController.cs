using Rekog.Controllers.Options;
using Rekog.Core;
using Rekog.Core.Ngrams;
using Rekog.IO;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class LayoutController : ControllerBase<LayoutOptions>
    {
        public LayoutController(LayoutOptions options, IConsole console, IFileSystem fileSystem) : base(options, console, fileSystem)
        {
        }

        public override async Task HandleAsync()
        {
            var layout = await ReadLayout();
            var ngrams = await ReadNgrams();

            var fingerNgrams = new Dictionary<Finger, List<Ngram>>();
            foreach ((var finger, var characters) in layout)
            {
                // Keep query Where order
                var query = ngrams.AsEnumerable();

                if (Options.TopRank.HasValue && Options.TopRank.Value > 0)
                {
                    query = query.Where(x => x.Rank <= Options.TopRank.Value);
                }
                if (Options.ThresholdPercentage.HasValue && Options.ThresholdPercentage.Value > 0)
                {
                    query = query.Where(x => x.Percentage >= Options.ThresholdPercentage.Value);
                }
                if (!Options.IncludeSameCharacter && ngrams.Size > 1)
                {
                    query = query.Where(x => x.Value.Distinct().Count() > 1);
                }

                query = query.Where(ngram => ngram.Value.All(x => characters.Any(c => char.ToUpper(c) == char.ToUpper(x))));

                fingerNgrams[finger] = query
                    .OrderBy(x => x.Rank)
                    .ToList();
            }

            await Write(fingerNgrams);
        }

        private async Task<Dictionary<Finger, HashSet<char>>> ReadLayout()
        {
            using (var dataReader = CreateDataReader(Options.Layout))
            using (var fingerCharactersReader = new LayoutReader(dataReader))
            {
                return await fingerCharactersReader.Read();
            }
        }

        private async Task<NgramCollection> ReadNgrams()
        {
            using (var dataReader = CreateDataReader(Options.Ngrams))
            using (var ngramsReader = new NgramCollectionReader(dataReader))
            {
                return await ngramsReader.Read();
            }
        }

        private async Task Write(Dictionary<Finger, List<Ngram>> fingerNgrams)
        {
            using (var dataWriter = CreateDataWriter(Options.Output))
            using (var writer = new FingerNgramsWriter(dataWriter))
            {
                await writer.Write(fingerNgrams);
            }
        }
    }
}
