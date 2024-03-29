﻿using System;
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

        public Direction GetHandRoll(Key other)
        {
            if (Hand != other.Hand || Math.Abs(other.Row - Row) > 1)
            {
                return Direction.None;
            }

            return (Hand, other.Finger - Finger) switch
            {
                (Hand.Left, 1) => Direction.Inward,
                (Hand.Left, -1) => Direction.Outward,
                (Hand.Right, 1) => Direction.Outward,
                (Hand.Right, -1) => Direction.Inward,
                _ => Direction.None,
            };
        }

        public Motion GetFingerMotion(Key other)
        {
            if (Finger != other.Finger || Finger.GetKind() == FingerKind.Thumb)
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
