using Rekog.Core.Ngrams;
using Rekog.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Rekog.Core.Corpora
{
    public class CorpusAnalyzer
    {
        private readonly NgramCollector _ignoredCollector;
        private readonly NgramCollector _unigramCollector;
        private readonly NgramCollector _bigramCollector;
        private readonly NgramCollector _trigramCollector;
        private readonly List<NgramCollector> _ngramCollectors;

        private readonly Alphabet _alphabet;
        private readonly bool _caseSensitive;
        private readonly bool _extended;

        public CorpusAnalyzer(Alphabet alphabet, bool caseSensitive, bool extended)
        {
            _alphabet = alphabet;
            _caseSensitive = caseSensitive;
            _extended = extended;

            _ignoredCollector = new NgramCollector(new UnigramBuffer());
            _unigramCollector = new NgramCollector(new UnigramBuffer());
            _bigramCollector = new NgramCollector(new NgramBuffer(2));
            _trigramCollector = new NgramCollector(new NgramBuffer(3));
            _ngramCollectors = new List<NgramCollector>
            {
                _unigramCollector,
                _bigramCollector,
                _trigramCollector,
            };
        }

        public void Append(CorpusAnalyzer other)
        {
            if (other == this)
            {
                throw new ArgumentException(nameof(other));
            }

            _ignoredCollector.Append(other._ignoredCollector);
            _unigramCollector.Append(other._unigramCollector);
            _bigramCollector.Append(other._bigramCollector);
            _trigramCollector.Append(other._trigramCollector);
        }

        public CorpusReport CreateReport()
        {
            return new CorpusReport
            {
                Unigrams = GetNgrams(_unigramCollector),
                Bigrams = GetNgrams(_bigramCollector),
                Trigrams = GetNgrams(_trigramCollector),
                Ignored = _extended ? GetNgrams(_ignoredCollector) : null,
            };

            static Dictionary<string, ulong> GetNgrams(NgramCollector collector)
            {
                return collector.GetNgrams().Values.OrderByDescending(x => x.Occurrences).ToDictionary(x => x.Value, x => x.Occurrences);
            }
        }

        public void Analyze(StreamReader reader, CancellationToken cancellationToken)
        {
            OnFile();

            Span<char> buffer = new char[4096];
            while (!cancellationToken.IsCancellationRequested)
            {
                var count = reader.Read(buffer);
                if (count == 0)
                {
                    break;
                }

                for (var i = 0; i < count; i++)
                {
                    OnCharacter(buffer[i]);
                }
            }
        }

        private void OnFile()
        {
            Skip();
        }

        private void OnCharacter(char character)
        {
            if (char.IsSurrogate(character))
            {
                Skip();
                return;
            }
            Next(character);
        }

        private void Skip()
        {
            if (_extended)
            {
                _ignoredCollector.Skip();
            }
            foreach (var ngramCollector in _ngramCollectors)
            {
                ngramCollector.Skip();
            }
        }

        private void Next(char character)
        {
            if (!_caseSensitive)
            {
                character = char.ToUpperInvariant(character);
            }

            if (_alphabet.Contains(character))
            {
                if (_extended)
                {
                    _ignoredCollector.Skip();
                }
                foreach (var ngramCollector in _ngramCollectors)
                {
                    ngramCollector.Next(character);
                }
            }
            else
            {
                if (_extended)
                {
                    _ignoredCollector.Next(character);
                }
                foreach (var ngramCollector in _ngramCollectors)
                {
                    ngramCollector.Skip();
                }
            }
        }
    }
}
