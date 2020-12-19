using Rekog.Core;
using Rekog.Core.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Extensions
{
    public class FingerExtensionsTests
    {
        [Theory]
        [InlineData(Finger.LeftPinky, Hand.Left)]
        [InlineData(Finger.LeftRing, Hand.Left)]
        [InlineData(Finger.LeftMiddle, Hand.Left)]
        [InlineData(Finger.LeftIndex, Hand.Left)]
        [InlineData(Finger.LeftThumb, Hand.Left)]
        [InlineData(Finger.RightPinky, Hand.Right)]
        [InlineData(Finger.RightRing, Hand.Right)]
        [InlineData(Finger.RightMiddle, Hand.Right)]
        [InlineData(Finger.RightIndex, Hand.Right)]
        [InlineData(Finger.RightThumb, Hand.Right)]
        public void GetHand(Finger finger, Hand expectedHand)
        {
            var hand = FingerExtensions.GetHand(finger);

            hand.ShouldBe(expectedHand);
        }

        [Theory]
        [MemberData(nameof(NeighborFingerList), parameters: true)]
        public void IsNeighbor_True(Finger first, Finger second)
        {
            var result = FingerExtensions.IsNeighbor(first, second);

            result.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(NeighborFingerList), parameters: false)]
        public void IsNeighbor_False(Finger first, Finger second)
        {
            var result = FingerExtensions.IsNeighbor(first, second);

            result.ShouldBeFalse();
        }

        public static IEnumerable<object[]> NeighborFingerList(bool isNeighbor)
        {
            var neighborsList = new HashSet<(Finger, Finger)>(new[]
            {
                (Finger.LeftPinky, Finger.LeftRing),
                (Finger.LeftRing, Finger.LeftMiddle),
                (Finger.LeftMiddle, Finger.LeftIndex),
                (Finger.RightPinky, Finger.RightRing),
                (Finger.RightRing, Finger.RightMiddle),
                (Finger.RightMiddle, Finger.RightIndex),
            }.SelectMany(x => new[] { (x.Item1, x.Item2), (x.Item2, x.Item1) }));

            if (isNeighbor)
            {
                foreach (var neighbors in neighborsList)
                {
                    yield return new object[] { neighbors.Item1, neighbors.Item2 };
                }
            }
            else
            {
                foreach (var first in Enum.GetValues<Finger>())
                {
                    foreach (var second in Enum.GetValues<Finger>())
                    {
                        var neighbors = (first, second);
                        if (!neighborsList.Contains(neighbors))
                        {
                            yield return new object[] { neighbors.Item1, neighbors.Item2 };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(RollFingerList), parameters: true)]
        public void GetRoll_True(Finger firstFinger, Finger secondFinger, Roll expectedRoll)
        {
            var hand = FingerExtensions.GetRoll(firstFinger, secondFinger);

            hand.ShouldBe(expectedRoll);
        }

        [Theory]
        [MemberData(nameof(RollFingerList), parameters: false)]
        public void GetRoll_False(Finger firstFinger, Finger secondFinger, Roll expectedRoll)
        {
            var hand = FingerExtensions.GetRoll(firstFinger, secondFinger);

            hand.ShouldBe(expectedRoll);
        }

        public static IEnumerable<object[]> RollFingerList(bool isRoll)
        {
            var rollList = new HashSet<(Finger, Finger, Roll)>(new[]
            {
                (Finger.LeftPinky, Finger.LeftRing, Roll.Inward),
                (Finger.LeftRing, Finger.LeftMiddle, Roll.Inward),
                (Finger.LeftMiddle, Finger.LeftIndex, Roll.Inward),
                (Finger.LeftIndex, Finger.LeftThumb, Roll.Inward),
                (Finger.LeftThumb, Finger.LeftIndex, Roll.Outward),
                (Finger.LeftIndex, Finger.LeftMiddle, Roll.Outward),
                (Finger.LeftMiddle, Finger.LeftRing, Roll.Outward),
                (Finger.LeftRing, Finger.LeftPinky, Roll.Outward),
                (Finger.RightPinky, Finger.RightRing, Roll.Inward),
                (Finger.RightRing, Finger.RightMiddle, Roll.Inward),
                (Finger.RightMiddle, Finger.RightIndex, Roll.Inward),
                (Finger.RightIndex, Finger.RightThumb, Roll.Inward),
                (Finger.RightThumb, Finger.RightIndex, Roll.Outward),
                (Finger.RightIndex, Finger.RightMiddle, Roll.Outward),
                (Finger.RightMiddle, Finger.RightRing, Roll.Outward),
                (Finger.RightRing, Finger.RightPinky, Roll.Outward),
            });

            if (isRoll)
            {
                foreach (var roll in rollList)
                {
                    yield return new object[] { roll.Item1, roll.Item2, roll.Item3 };
                }
            }
            else
            {
                foreach (var first in Enum.GetValues<Finger>())
                {
                    foreach (var second in Enum.GetValues<Finger>())
                    {
                        if (!rollList.Any(x => x.Item1 == first && x.Item2 == second))
                        {
                            yield return new object[] { first, second, Roll.None };
                        }
                    }
                }
            }
        }
    }
}
