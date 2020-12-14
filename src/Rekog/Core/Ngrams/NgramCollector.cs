namespace Rekog.Core.Ngrams
{
    public class NgramCollector
    {
        private readonly INgramParser _parser;

        public NgramCollector(INgramParser parser)
        {
            _parser = parser;
        }

        public OccurrenceCollection<string> Occurrences { get; } = new();

        public void Skip()
        {
            _parser.Skip();
        }

        public void Next(char character)
        {
            if (_parser.Next(character, out var ngramValue))
            {
                Occurrences.Add(ngramValue, 1);
            }
        }
    }
}
