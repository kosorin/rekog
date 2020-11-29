using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Ngrams
{
    public interface INgramBuffer
    {
        void Skip();

        bool Next(char character, [MaybeNullWhen(false)] out string ngramValue);
    }
}