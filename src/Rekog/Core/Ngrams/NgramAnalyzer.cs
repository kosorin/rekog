using Rekog.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.Core.Ngrams
{
    public class NgramAnalyzer
    {
        private readonly INgramScanner _scanner;
        private readonly Dictionary<string, RawNgram> _rawData;

        public NgramAnalyzer(int size, bool caseSensitive, Alphabet alphabet)
        {
            _scanner = new NgramScannerFactory().Create(size, caseSensitive, alphabet);
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
            _scanner.Clear();
        }

        private void OnCharacter(char character)
        {
            if (_scanner.Next(character, out var ngramValue))
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
