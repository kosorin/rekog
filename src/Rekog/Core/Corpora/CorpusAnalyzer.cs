using Rekog.Core.Ngrams;
using Rekog.Extensions;
using Rekog.Persistence;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Rekog.Core.Corpora
{
    public class CorpusAnalyzer
    {
        private readonly NgramCollector _unigramCollector;
        private readonly NgramCollector _bigramCollector;
        private readonly NgramCollector _trigramCollector;
        private readonly List<NgramCollector> _ngramCollectors;
        private readonly NgramCollector? _ignoredCollector;

        private readonly Alphabet _alphabet;
        private readonly bool _caseSensitive;

        public CorpusAnalyzer(Alphabet alphabet, bool caseSensitive, bool includeIgnored)
        {
            _alphabet = alphabet;
            _caseSensitive = caseSensitive;

            _unigramCollector = new NgramCollector(new UnigramBuffer());
            _bigramCollector = new NgramCollector(new NgramBuffer(2));
            _trigramCollector = new NgramCollector(new NgramBuffer(3));
            _ngramCollectors = new List<NgramCollector>
            {
                _unigramCollector,
                _bigramCollector,
                _trigramCollector,
            };
            _ignoredCollector = includeIgnored ? new NgramCollector(new UnigramBuffer()) : null;
        }

        public CorpusReport CreateReport()
        {
            return new CorpusReport
            {
                Unigrams = GetNgrams(_unigramCollector.GetNgrams()),
                Bigrams = GetNgrams(_bigramCollector.GetNgrams()),
                Trigrams = GetNgrams(_trigramCollector.GetNgrams()),
                Ignored = _ignoredCollector != null ? GetNgrams(_ignoredCollector.GetNgrams()) : null,
            };

            static Dictionary<string, ulong> GetNgrams(NgramCollection ngrams)
            {
                return ngrams.Values.OrderByDescending(x => x.Occurrences).ToDictionary(x => x.Value, x => x.Occurrences);
            }
        }

        public void Analyze(IFileSystem fileSystem, CorpusFile file)
        {
            Skip();

            using (var reader = file.Open(fileSystem))
            {
                while (reader.TryReadChar(out var character))
                {
                    if (char.IsSurrogate(character))
                    {
                        Skip();
                        continue;
                    }

                    Next(character);
                }
            }
        }

        private void Skip()
        {
            _ignoredCollector?.Skip();
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
                _ignoredCollector?.Skip();
                foreach (var ngramCollector in _ngramCollectors)
                {
                    ngramCollector.Next(character);
                }
            }
            else
            {
                _ignoredCollector?.Next(character);
                foreach (var ngramCollector in _ngramCollectors)
                {
                    ngramCollector.Skip();
                }
            }
        }
    }
}
