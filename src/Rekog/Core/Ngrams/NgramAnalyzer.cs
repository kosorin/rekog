using Rekog.Core.Ngrams;
using Rekog.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rekog.Core.Ngram
{
    public class NgramAnalyzer
    {
        private readonly NgramParser _parser;
        private readonly Dictionary<string, RawNgram> _rawData;

        public NgramAnalyzer(int size, bool caseSensitive, Alphabet alphabet)
        {
            _parser = new NgramParser(size, caseSensitive, alphabet);
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
            _parser.Clear();
        }

        private void OnCharacter(char character)
        {
            if (_parser.Next(character, out var ngramValue))
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
