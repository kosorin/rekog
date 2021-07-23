using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rekog.Core.Corpora
{
    public interface INgramParser
    {
        int Size { get; }

        void Skip();

        bool Next(Rune character, [MaybeNullWhen(false)] out string ngramValue);
    }
}
