using System.Collections.Generic;

namespace Rekog.Core.Layouts
{
    public class Layout
    {
        private readonly Dictionary<char, Finger> _characterFingers;
        private readonly Dictionary<char, Hand> _handFingers;

        public int RowCount { get; }

        public Finger GetFinger(char character)
        {
            return _characterFingers[character];
        }

        public Hand GetHand(char character)
        {
            return _handFingers[character];
        }

        public int GetRow(char character)
        {
            return 0;
        }
    }
}
