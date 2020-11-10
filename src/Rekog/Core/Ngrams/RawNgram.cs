namespace Rekog.Core.Ngrams
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    // TODO: RawNgram is probably useless
    public class RawNgram
    {
        public string Value { get; set; }

        public ulong Occurrences { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}
