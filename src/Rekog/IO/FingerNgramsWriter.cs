using Rekog.Core;
using Rekog.Core.Ngrams;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekog.IO
{
    public class FingerNgramsWriter : DataWriter
    {
        public FingerNgramsWriter(IDataWriter writer) : base(writer)
        {
        }

        public async Task Write(Dictionary<Finger, List<Ngram>> fingerNgrams)
        {
            var data = fingerNgrams
                .SelectMany(x => x.Value.Select(ngram => (finger: x.Key, ngram)))
                .OrderBy(x => x.ngram.Rank);
            foreach ((var finger, var ngram) in data)
            {
                var sb = new StringBuilder();
                sb.Append((int)finger);
                sb.Append(FileFormat.Delimiter);
                sb.Append(ngram.Value);
                sb.Append(FileFormat.Delimiter);
                sb.Append(ngram.Rank);
                sb.Append(FileFormat.Delimiter);
                sb.Append(ngram.Percentage.ToString("N5", FileFormat.CultureInfo));
                await Writer.WriteLineAsync(sb);
            }
        }
    }
}
