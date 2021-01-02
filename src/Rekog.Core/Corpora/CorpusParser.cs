using Rekog.Core.Ngrams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Rekog.Core.Corpora
{
    public class CorpusParser
    {
        private readonly NgramCollector _unigramCollector;
        private readonly NgramCollector _bigramCollector;
        private readonly NgramCollector _trigramCollector;
        private readonly List<ITokenCollector> _tokenCollectors;

        private readonly Alphabet _alphabet;
        private readonly bool _caseSensitive;

        public CorpusParser(Alphabet alphabet)
        {
            _alphabet = alphabet;
            _caseSensitive = false;

            _unigramCollector = new NgramCollector(new UnigramParser());
            _bigramCollector = new NgramCollector(new NgramParser(2));
            _trigramCollector = new NgramCollector(new NgramParser(3));
            _tokenCollectors = new List<ITokenCollector>
            {
                _unigramCollector,
                _bigramCollector,
                _trigramCollector,
            };
        }

        public CorpusAnalysisData GetAnalysisData()
        {
            return new CorpusAnalysisData(_unigramCollector.Occurrences, _bigramCollector.Occurrences, _trigramCollector.Occurrences);
        }

        public void Parse(StreamReader reader, CancellationToken cancellationToken)
        {
            Span<char> buffer = new char[4096];
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var count = reader.Read(buffer);
                if (count == 0)
                {
                    break;
                }

                for (var i = 0; i < count; i++)
                {
                    Next(buffer[i]);
                }
            }

            Skip();
        }

        private void Skip()
        {
            foreach (var tokenCollector in _tokenCollectors)
            {
                tokenCollector.Skip();
            }
        }

        private void Next(char character)
        {
            if (_alphabet.Contains(character))
            {
                if (!_caseSensitive)
                {
                    character = char.ToUpperInvariant(character);
                }

                foreach (var tokenCollector in _tokenCollectors)
                {
                    tokenCollector.Next(character);
                }
            }
            else
            {
                Skip();
            }
        }
    }
}
