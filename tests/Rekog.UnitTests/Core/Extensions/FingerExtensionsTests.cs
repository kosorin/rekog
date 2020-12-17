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
        public void ToHand(Finger finger, Hand expectedHand)
        {
            var hand = FingerExtensions.ToHand(finger);

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
    }
}
