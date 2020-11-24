namespace Rekog.Core.Ngrams
{
    public interface INgramBuffer
    {
        void Skip();

        bool Next(char character, out string ngramValue);
    }
}