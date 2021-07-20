using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Serilog;

namespace Rekog.Core.Corpora
{
    public class CorpusAnalyzer
    {
        private readonly OccurrenceCollection<Rune> _replacedCharacters = new OccurrenceCollection<Rune>();
        private readonly OccurrenceCollection<Rune> _skippedCharacters = new OccurrenceCollection<Rune>();
        private readonly NgramCollector _unigramCollector;
        private readonly NgramCollector _bigramCollector;
        private readonly NgramCollector _trigramCollector;
        private readonly List<ITokenCollector> _tokenCollectors;

        private readonly ILogger _logger;

        private readonly Alphabet _alphabet;
        private readonly bool _simplifyUnicode;
        private readonly bool _caseSensitive;

        public CorpusAnalyzer(Alphabet alphabet, ILogger logger)
        {
            _logger = logger;

            _alphabet = alphabet;
            _simplifyUnicode = true;
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
            return new CorpusAnalysisData(_unigramCollector.Occurrences, _bigramCollector.Occurrences, _trigramCollector.Occurrences, _replacedCharacters, _skippedCharacters);
        }

        public void AnalyzeNext(StreamReader reader, CancellationToken cancellationToken)
        {
            var characterReader = new CharacterReader(_logger.ForContext<CharacterReader>());

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
                    if (!characterReader.ReadNext(buffer[i], out var character))
                    {
                        // Don't Skip() here, there is pending high surrogate!
                        continue;
                    }

                    if (!Next(character))
                    {
                        Skip();
                    }
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

        private bool Next(Rune character)
        {
            var contains = _alphabet.Contains(character);

            if (!contains && _simplifyUnicode)
            {
                if (UnicodeHelper.TrySimplify(character, out var simplifiedCharacter))
                {
                    _replacedCharacters.Add(character);

                    if (_alphabet.Contains(simplifiedCharacter))
                    {
                        contains = true;
                        character = simplifiedCharacter;
                    }
                }
            }

            if (!contains)
            {
                _skippedCharacters.Add(character);
                return false;
            }

            if (!_caseSensitive)
            {
                character = Rune.ToUpperInvariant(character);
            }

            foreach (var tokenCollector in _tokenCollectors)
            {
                tokenCollector.Next(character);
            }

            return true;
        }
    }
}
