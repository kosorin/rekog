﻿using Rekog.Core.Extensions;
using System;

namespace Rekog.Core.Layouts
{
    public class Key
    {
        public Key(char character, Finger finger, bool isHoming, double effort, int layer, int row, int column)
        {
            Character = character;
            Finger = finger;
            Hand = finger.GetHand();
            Effort = effort;
            Layer = layer;
            Row = row;
            Column = column;
            Position = (row, column);
            IsHoming = isHoming;
        }

        public char Character { get; }

        public Finger Finger { get; }

        public Hand Hand { get; }

        public bool IsHoming { get; }

        public double Effort { get; }

        public int Layer { get; }

        public int Row { get; }

        public int Column { get; }

        public (int row, int column) Position { get; }

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

        public Motion GetHandMotion(Key other)
        {
            if (other.Hand != Hand)
            {
                return Motion.None;
            }

            return (other.Row - Row) switch
            {
                > 0 => Motion.Curl,
                < 0 => Motion.Stretch,
                _ => Motion.None
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
                _ => Motion.None
            };
        }
    }
}