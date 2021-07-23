using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rekog.Core.Layouts
{
    public class Layout
    {
        private readonly Dictionary<Rune, Key> _keys;

        public Layout(IReadOnlyDictionary<Rune, Key> keys)
        {
            _keys = keys.ToDictionary(x => x.Key, x => x.Value);
        }

        public OccurrenceCollection<LayoutNgram> GetNgramOccurrences(OccurrenceCollection<string> ngramOccurrences)
        {
            var layoutNgramOccurrences = new OccurrenceCollection<LayoutNgram>();
            foreach (var (ngram, count) in ngramOccurrences)
            {
                var ngramKeys = GetNgramKeys(ngram);
                if (ngramKeys.Length == ngram.Length)
                {
                    layoutNgramOccurrences.Add(new LayoutNgram(ngram, ngramKeys), count);
                }
                else
                {
                    layoutNgramOccurrences.AddNull(count);
                }
            }
            return layoutNgramOccurrences;
        }

        private Key[] GetNgramKeys(string ngram)
        {
            return ngram
                .EnumerateRunes()
                .Select(character => _keys.TryGetValue(character, out var key) ? key : null)
                .Where(x => x != null)
                .Select(x => x!)
                .ToArray();
        }
    }
}
