using System;

namespace Rekog.Core.Ngrams
{
    public class NgramScannerFactory
    {
        public INgramScanner Create(int size, bool caseSensitive, Alphabet alphabet)
        {
            return size switch
            {
                1 => new UnigramScanner(caseSensitive, alphabet),
                var n when n > 1 => new NgramScanner(size, caseSensitive, alphabet),
                _ => throw new ArgumentOutOfRangeException(nameof(size)),
            };
        }
    }
}
