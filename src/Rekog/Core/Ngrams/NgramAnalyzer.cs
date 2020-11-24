using Rekog.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.Core.Ngrams
{
    public class NgramAnalyzer
    {
        private readonly INgramBuffer _buffer;
        private readonly Dictionary<string, RawNgram> _rawData;

        public NgramAnalyzer(int size)
        {
            _buffer = new NgramBufferFactory().Create(size);
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

        public async Task AnalyzeNext(IDataReader dataReader)
        {
            while (true)
            {
                var line = await dataReader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                OnNewLine();

                foreach (var character in line)
                {
                    OnCharacter(character);
                }
            }
        }

        private void OnNewLine()
        {
            _buffer.Skip();
        }

        private void OnCharacter(char character)
        {
            if (_buffer.Next(character, out var ngramValue))
            {
                if (!_rawData.TryGetValue(ngramValue, out var rawNgram))
                {
                    _rawData.Add(ngramValue, rawNgram = new RawNgram(ngramValue));
                }
                rawNgram.Occurrences++;
            }
        }
    }
}
