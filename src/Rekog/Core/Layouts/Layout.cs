using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core.Layouts
{
    public class Layout
    {
        private readonly Dictionary<char, Key> _keys;

        public Layout(IReadOnlyDictionary<char, Key> keys)
        {
            _keys = keys.ToDictionary(x => x.Key, x => x.Value);
        }

        public bool TryGetKey(char character, [MaybeNullWhen(false)] out Key key)
        {
            return _keys.TryGetValue(character, out key);
        }
    }
}
