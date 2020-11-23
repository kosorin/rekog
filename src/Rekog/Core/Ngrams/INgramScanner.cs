namespace Rekog.Core.Ngrams
{
    public interface INgramScanner
    {
        int Size { get; }

        bool CaseSensitive { get; }

        Alphabet Alphabet { get; }

        void Clear();

        bool Next(char character, out string ngramValue);
    }
}