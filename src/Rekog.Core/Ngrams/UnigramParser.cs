using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Ngrams
{
    public class UnigramParser : INgramParser
    {
        public void Skip()
        {
            // Nothing to do
        }

        public bool Next(char character, [MaybeNullWhen(false)] out string ngramValue)
        {
            ngramValue = character.ToString();
            return true;
        }
    }
}
