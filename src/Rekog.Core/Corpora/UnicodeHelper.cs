using System.Collections.Generic;
using System.Text;

namespace Rekog.Core.Corpora
{
    public static class UnicodeHelper
    {
        private static readonly Dictionary<Rune, Rune> SimplifyTable = new Dictionary<Rune, Rune>
        {
            [(Rune)'\u00b4'] = (Rune)'\'',
            [(Rune)'\u02b9'] = (Rune)'\'',
            [(Rune)'\u02bc'] = (Rune)'\'',
            [(Rune)'\u02c8'] = (Rune)'\'',
            [(Rune)'\u0301'] = (Rune)'\'',
            [(Rune)'\u2018'] = (Rune)'\'',
            [(Rune)'\u2019'] = (Rune)'\'',
            [(Rune)'\u201a'] = (Rune)'\'',
            [(Rune)'\u201b'] = (Rune)'\'',
            [(Rune)'\u2039'] = (Rune)'\'',
            [(Rune)'\u203a'] = (Rune)'\'',
            [(Rune)'\u276e'] = (Rune)'\'',
            [(Rune)'\u276f'] = (Rune)'\'',
            [(Rune)'\u00ab'] = (Rune)'"',
            [(Rune)'\u00bb'] = (Rune)'"',
            [(Rune)'\u02ba'] = (Rune)'"',
            [(Rune)'\u030b'] = (Rune)'"',
            [(Rune)'\u030e'] = (Rune)'"',
            [(Rune)'\u201c'] = (Rune)'"',
            [(Rune)'\u201d'] = (Rune)'"',
            [(Rune)'\u201e'] = (Rune)'"',
            [(Rune)'\u201f'] = (Rune)'"',
            [(Rune)'\u2033'] = (Rune)'"',
            [(Rune)'\u2036'] = (Rune)'"',
            [(Rune)'\u2e42'] = (Rune)'"',
            [(Rune)'\u3003'] = (Rune)'"',
            [(Rune)'\u301d'] = (Rune)'"',
            [(Rune)'\u301e'] = (Rune)'"',
            [(Rune)'\u301f'] = (Rune)'"',
            [(Rune)'\uff02'] = (Rune)'"',
            [(Rune)'\u058a'] = (Rune)'-',
            [(Rune)'\u05be'] = (Rune)'-',
            [(Rune)'\u1806'] = (Rune)'-',
            [(Rune)'\u2010'] = (Rune)'-',
            [(Rune)'\u2011'] = (Rune)'-',
            [(Rune)'\u2012'] = (Rune)'-',
            [(Rune)'\u2013'] = (Rune)'-',
            [(Rune)'\u2014'] = (Rune)'-',
            [(Rune)'\u2015'] = (Rune)'-',
            [(Rune)'\u2043'] = (Rune)'-',
            [(Rune)'\u2e3a'] = (Rune)'-',
            [(Rune)'\u2e3b'] = (Rune)'-',
            [(Rune)'\ufe58'] = (Rune)'-',
            [(Rune)'\ufe63'] = (Rune)'-',
            [(Rune)'\uff0d'] = (Rune)'-',
            [(Rune)'\u005f'] = (Rune)'_',
        };

        public static bool TrySimplify(Rune character, out Rune simplifiedCharacter)
        {
            return SimplifyTable.TryGetValue(character, out simplifiedCharacter);
        }
    }
}
