using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Corpora
{
    public interface INgramParser
    {
        void Skip();

        bool Next(char character, [MaybeNullWhen(false)] out string ngramValue);
    }
}
