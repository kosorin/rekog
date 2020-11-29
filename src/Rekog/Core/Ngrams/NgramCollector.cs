using System.Collections.Generic;

namespace Rekog.Core.Ngrams
{
    public class NgramCollector
    {
        private readonly INgramBuffer _buffer;
        private readonly Dictionary<string, RawNgram> _rawNgrams;

        public NgramCollector(INgramBuffer buffer)
        {
            _buffer = buffer;
            _rawNgrams = new Dictionary<string, RawNgram>();
        }

        public void Append(NgramCollector other)
        {
            foreach (var otherRawNgram in other._rawNgrams.Values)
            {
                AddNgramValue(otherRawNgram.Value, otherRawNgram.Occurrences);
            }
        }

        public NgramCollection GetNgrams()
        {
            return new NgramCollection(_rawNgrams.Values);
        }

        public void Skip()
        {
            _buffer.Skip();
        }

        public void Next(char character)
        {
            if (_buffer.Next(character, out var ngramValue))
            {
                AddNgramValue(ngramValue, 1);
            }
        }

        private void AddNgramValue(string ngramValue, ulong occurrences)
        {
            if (!_rawNgrams.TryGetValue(ngramValue, out var rawNgram))
            {
                _rawNgrams.Add(ngramValue, rawNgram = new RawNgram(ngramValue));
            }
            rawNgram.Occurrences += occurrences;
        }
    }
}
