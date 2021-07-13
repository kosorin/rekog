namespace Rekog.Core.Corpora
{
    public interface ITokenCollector
    {
        OccurrenceCollection<string> Occurrences { get; }

        void Skip();

        void Next(char character);
    }
}
