using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rekog.Core.Corpora
{
    public class UnigramParser : INgramParser
    {
        private static readonly ConcurrentDictionary<Rune, string> NgramValueCache = new ConcurrentDictionary<Rune, string>();

        public int Size => 1;

        public void Skip()
        {
            // Nothing to do
        }

        public bool Next(Rune character, [MaybeNullWhen(false)] out string ngramValue)
        {
            ngramValue = GetCachedNgramValue(character);

            // Always return true
            return true;
        }

        private static string GetCachedNgramValue(Rune character)
        {
            if (NgramValueCache.TryGetValue(character, out var ngramValue))
            {
                return ngramValue;
            }

            ngramValue = character.ToString();
            NgramValueCache.TryAdd(character, ngramValue);

            return ngramValue;
        }
    }
}
