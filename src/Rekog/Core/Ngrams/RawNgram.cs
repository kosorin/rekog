namespace Rekog.Core.Ngrams
{
    public class RawNgram
    {
        public RawNgram(string value)
        {
            Value = value;
        }

        public RawNgram(string value, ulong occurrences) : this(value)
        {
            Occurrences = occurrences;
        }

        public string Value { get; }

        public ulong Occurrences { get; set; }
    }
}
