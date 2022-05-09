using System.Collections.Generic;

namespace Rekog.App.Kle
{
    public class KleBoard
    {
        public List<KleKey> Keys { get; set; } = new List<KleKey>();

        public string Background { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}
