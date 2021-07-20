using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rekog.Core.Corpora
{
    public interface INgramParser
    {
        void Skip();

        bool Next(Rune character, [MaybeNullWhen(false)] out string ngramValue);
    }
}
