using Rekog.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rekog.Commands.Ngram
{
    public class NgramAnalyzer
    {
        private readonly int _size;
        private readonly bool _caseSensitive;
        private readonly string? _alphabet;
        private readonly Dictionary<string, decimal> _rawData = new Dictionary<string, decimal>();

        public NgramAnalyzer(int size, bool caseSensitive, string? alphabet)
        {
            _size = size;
            _caseSensitive = caseSensitive;
            _alphabet = alphabet;
        }

        public NgramStatistics GetStatistics()
        {
            return new NgramStatistics(_rawData);
        }

        public async Task Analyze(StreamReader reader)
        {
            var buffer = new NgramAnalyzeBuffer(_size, _caseSensitive, _alphabet);
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                buffer.Add('\0');

                foreach (var ch in line)
                {
                    var ngram = buffer.Add(ch);
                    if (ngram != null)
                    {
                        if (_rawData.ContainsKey(ngram))
                        {
                            _rawData[ngram]++;
                        }
                        else
                        {
                            _rawData[ngram] = 1;
                        }
                    }
                }
            }
        }
    }
}
