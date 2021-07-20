using System.Text;
using Serilog;

namespace Rekog.Core.Corpora
{
    public class CharacterReader
    {
        private readonly ILogger _logger;

        private char? _pendingHighSurrogate;

        public CharacterReader(ILogger logger)
        {
            _logger = logger;
        }

        public bool ReadNext(char c, out Rune character)
        {
            if (char.IsSurrogate(c))
            {
                if (_pendingHighSurrogate.HasValue)
                {
                    if (char.IsLowSurrogate(c))
                    {
                        character = new Rune(_pendingHighSurrogate.Value, c);
                        _pendingHighSurrogate = null;
                        return true;
                    }
                    else
                    {
                        _logger.Warning("Multiple high surrogates.");
                        _pendingHighSurrogate = c;
                    }
                }
                else
                {
                    if (char.IsHighSurrogate(c))
                    {
                        _pendingHighSurrogate = c;
                    }
                    else
                    {
                        _logger.Warning("Missing high surrogate.");
                        _pendingHighSurrogate = null;
                    }
                }

                character = Rune.ReplacementChar;
                return false;
            }

            if (_pendingHighSurrogate.HasValue)
            {
                _logger.Warning("Missing low surrogate.");
                _pendingHighSurrogate = null;
            }

            character = new Rune(c);
            return true;
        }
    }
}
