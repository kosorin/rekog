using System.Text;

namespace Rekog.Core.Corpora
{
    public class NgramCollector : ITokenCollector
    {
        private readonly INgramParser _parser;

        public NgramCollector(INgramParser parser)
        {
            _parser = parser;
        }

        public OccurrenceCollection<string> Occurrences { get; } = new OccurrenceCollection<string>();

        public void Skip()
        {
            _parser.Skip();
        }

        public void Next(Rune character)
        {
            if (_parser.Next(character, out var ngramValue))
            {
                Occurrences.Add(ngramValue);
            }
        }
    }
}
