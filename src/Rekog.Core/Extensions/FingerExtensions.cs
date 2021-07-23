using System;

namespace Rekog.Core.Extensions
{
    public static class FingerExtensions
    {
        public static FingerKind GetKind(this Finger finger)
        {
            switch (finger)
            {
                case Finger.LeftLittle:
                case Finger.RightLittle:
                    return FingerKind.Little;
                case Finger.LeftRing:
                case Finger.RightRing:
                    return FingerKind.Ring;
                case Finger.LeftMiddle:
                case Finger.RightMiddle:
                    return FingerKind.Middle;
                case Finger.LeftIndex:
                case Finger.RightIndex:
                    return FingerKind.Index;
                case Finger.LeftThumb:
                case Finger.RightThumb:
                    return FingerKind.Thumb;
                default: throw new ArgumentOutOfRangeException(nameof(finger));
            }
        }

        public static Hand GetHand(this Finger finger)
        {
            switch (finger)
            {
                case Finger.LeftLittle:
                case Finger.LeftRing:
                case Finger.LeftMiddle:
                case Finger.LeftIndex:
                case Finger.LeftThumb:
                    return Hand.Left;
                case Finger.RightLittle:
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
            if (finger.GetKind() == FingerKind.Thumb || neighbor.GetKind() == FingerKind.Thumb)
            {
                return false;
            }

            var distance = Math.Abs(neighbor - finger);
            return distance == 1;
        }
    }
}
