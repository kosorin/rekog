using Rekog.Core.Ngrams;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rekog.Core.Ngram
{
    public class NgramAnalyzer
    {
        private readonly NgramAnalyzerBuffer _buffer;
        private readonly Dictionary<string, RawNgram> _rawData;

        public NgramAnalyzer(int size, bool caseSensitive, Alphabet alphabet)
        {
            _buffer = new NgramAnalyzerBuffer(size, caseSensitive, alphabet);
            _rawData = new Dictionary<string, RawNgram>();
        }

        public void Clear()
        {
            _rawData.Clear();
        }

        public NgramCollection CreateResult()
        {
            return new NgramCollection(_rawData.Values);
        }

        public async Task AnalyzeNext(TextReader reader)
        {
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                OnNewLine();

                foreach (var character in line)
                {
                    OnNextCharacter(character);
                }
            }
        }

        private void OnNewLine()
        {
            _buffer.Clear();
        }

        private void OnNextCharacter(char character)
        {
            if (_buffer.Next(character, out var ngramValue))
            {
                if (_rawData.TryGetValue(ngramValue, out var rawNgram))
                {
                    rawNgram.Occurrences++;
                }
                else
                {
                    _rawData[ngramValue] = new RawNgram
                    {
                        Value = ngramValue,
                        Occurrences = 1
                    };
                }
            }
        }
    }
}
