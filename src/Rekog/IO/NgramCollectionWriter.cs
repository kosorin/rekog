using Rekog.Core.Ngrams;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class NgramCollectionWriter : WriterBase
    {
        private readonly bool _raw;

        public NgramCollectionWriter(IDataWriter writer, bool raw) : base(writer)
        {
            _raw = raw;
        }

        public Task Write(NgramCollection ngrams)
        {
            return Write(ngrams, int.MaxValue);
        }

        public async Task Write(NgramCollection ngrams, int topCount)
        {
            foreach (var ngram in ngrams.OrderBy(x => x.Rank).Take(topCount))
            {
                var sb = new StringBuilder();
                sb.Append(ngram.Value);
                sb.Append(FileFormat.Delimiter);
                sb.Append(ngram.Occurrences);
                if (!_raw)
                {
                    sb.Append(FileFormat.Delimiter);
                    sb.Append(ngram.Rank);
                    sb.Append(FileFormat.Delimiter);
                    sb.Append(ngram.Percentage.ToString("N5", CultureInfo.InvariantCulture));
                }
                await Writer.WriteLineAsync(sb);
            }
        }
    }
}
