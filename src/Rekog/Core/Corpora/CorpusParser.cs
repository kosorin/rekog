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
        private readonly List<NgramCollector> _ngramCollectors;

        private readonly Alphabet _alphabet;
        private readonly bool _caseSensitive;

        public CorpusParser(Alphabet alphabet, bool caseSensitive)
        {
            _alphabet = alphabet;
            _caseSensitive = caseSensitive;

            _unigramCollector = new NgramCollector(new UnigramParser());
            _bigramCollector = new NgramCollector(new NgramParser(2));
            _trigramCollector = new NgramCollector(new NgramParser(3));
            _ngramCollectors = new List<NgramCollector>
            {
                _unigramCollector,
                _bigramCollector,
                _trigramCollector,
            };
        }

        public CorpusData GetData()
        {
            return new CorpusData(_unigramCollector.Occurrences, _bigramCollector.Occurrences, _trigramCollector.Occurrences);
        }

        public void Parse(StreamReader reader, CancellationToken cancellationToken)
        {
            Skip();

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
        }

        private void Skip()
        {
            foreach (var ngramCollector in _ngramCollectors)
            {
                ngramCollector.Skip();
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

                foreach (var ngramCollector in _ngramCollectors)
                {
                    ngramCollector.Next(character);
                }
            }
            else
            {
                Skip();
            }
        }
    }
}
