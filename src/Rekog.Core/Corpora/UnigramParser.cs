using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Corpora
{
    public class UnigramParser : INgramParser
    {
        private static readonly ConcurrentDictionary<char, string> NgramValueCache = new ConcurrentDictionary<char, string>();

        public void Skip()
        {
            // Nothing to do
        }

        public bool Next(char character, [MaybeNullWhen(false)] out string ngramValue)
        {
            ngramValue = GetCachedNgramValue(character);

            // Always return true
            return true;
        }

        private static string GetCachedNgramValue(char character)
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
