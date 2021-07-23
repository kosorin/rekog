using System;
using System.Text;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts
{
    public class Key
    {
        public Key(Rune character, Finger finger, bool isHoming, int layer, int row, int column, double effort)
        {
            Character = character;
            Finger = finger;
            Hand = finger.GetHand();
            Layer = layer;
            Row = row;
            Column = column;
            Position = (row, column);
            IsHoming = isHoming;
            Effort = effort;
        }

        public Rune Character { get; }

        public Finger Finger { get; }

        public Hand Hand { get; }

        public bool IsHoming { get; }

        public int Layer { get; }

        public int Row { get; }

        public int Column { get; }

        public (int row, int column) Position { get; }

        public double Effort { get; }

        public double GetDistance(Key other)
        {
            return Math.Ceiling(Math.Sqrt(Math.Pow(other.Row - Row, 2) + Math.Pow(other.Column - Column, 2)));
        }

        public Roll GetHandRoll(Key other)
        {
            if (other.Hand != Hand || Math.Abs(other.Row - Row) > 1)
            {
                return Roll.None;
            }

            return (Hand, other.Finger - Finger) switch
            {
                (Hand.Left, 1) => Roll.Inward,
                (Hand.Left, -1) => Roll.Outward,
                (Hand.Right, 1) => Roll.Outward,
                (Hand.Right, -1) => Roll.Inward,
                _ => Roll.None,
            };
        }

        public Motion GetFingerMotion(Key other)
        {
            if (other.Finger != Finger)
            {
                return Motion.None;
            }

            return (other.Row - Row) switch
            {
                > 0 => Motion.Curl,
                < 0 => Motion.Stretch,
                _ => Motion.None,
            };
        }
    }
}
