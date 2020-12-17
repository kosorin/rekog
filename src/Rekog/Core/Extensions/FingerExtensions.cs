using System;

namespace Rekog.Core.Extensions
{
    public static class FingerExtensions
    {
        public static Hand ToHand(this Finger finger)
        {
            switch (finger)
            {
            case Finger.LeftPinky:
            case Finger.LeftRing:
            case Finger.LeftMiddle:
            case Finger.LeftIndex:
            case Finger.LeftThumb:
                return Hand.Left;
            case Finger.RightPinky:
            case Finger.RightRing:
            case Finger.RightMiddle:
            case Finger.RightIndex:
            case Finger.RightThumb:
                return Hand.Right;
            default: throw new ArgumentOutOfRangeException(nameof(finger));
            }
        }

        public static bool IsNeighbor(this Finger finger, Finger neighbor)
        {
            if (finger == Finger.LeftThumb || finger == Finger.RightThumb || neighbor == Finger.LeftThumb || neighbor == Finger.RightThumb)
            {
                return false;
            }

            var distance = neighbor - finger;
            return distance == 1 || distance == -1;
        }
    }
}
