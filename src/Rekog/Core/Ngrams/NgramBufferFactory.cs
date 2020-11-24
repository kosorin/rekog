using System;

namespace Rekog.Core.Ngrams
{
    public class NgramBufferFactory
    {
        public INgramBuffer Create(int size)
        {
            return size switch
            {
                1 => new UnigramBuffer(),
                var n when n > 1 => new NgramBuffer(size),
                _ => throw new ArgumentOutOfRangeException(nameof(size)),
            };
        }
    }
}
