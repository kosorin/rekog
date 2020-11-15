namespace Rekog.Core.Ngrams
{
    // TODO: RawNgram is probably not necessary
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
