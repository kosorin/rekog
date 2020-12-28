using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Ngrams
{
    public interface INgramParser
    {
        void Skip();

        bool Next(char character, [MaybeNullWhen(false)] out string ngramValue);
    }
}