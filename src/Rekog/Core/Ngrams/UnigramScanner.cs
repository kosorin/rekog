namespace Rekog.Core.Ngrams
{
    public class UnigramScanner : INgramScanner
    {
        public UnigramScanner(bool caseSensitive, Alphabet alphabet)
        {
            CaseSensitive = caseSensitive;
            Alphabet = alphabet;
        }

        public int Size => 1;

        public bool CaseSensitive { get; }

        public Alphabet Alphabet { get; }

        public void Clear()
        {
            // Nothing to do
        }

        public bool Next(char character, out string ngramValue)
        {
            if (Alphabet.Contains(character))
            {
                ngramValue = (CaseSensitive ? character : char.ToUpperInvariant(character)).ToString();
                return true;
            }
            else
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                ngramValue = null;
                return false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
    }
}
