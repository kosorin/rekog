using System;
using System.Collections.Generic;
using System.Linq;
using Rekog.Core.Extensions;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Extensions
{
    public class FingerExtensionsTests
    {
        [Theory]
        [InlineData(Finger.LeftLittle, Hand.Left)]
        [InlineData(Finger.LeftRing, Hand.Left)]
        [InlineData(Finger.LeftMiddle, Hand.Left)]
        [InlineData(Finger.LeftIndex, Hand.Left)]
        [InlineData(Finger.LeftThumb, Hand.Left)]
        [InlineData(Finger.RightLittle, Hand.Right)]
        [InlineData(Finger.RightRing, Hand.Right)]
        [InlineData(Finger.RightMiddle, Hand.Right)]
        [InlineData(Finger.RightIndex, Hand.Right)]
        [InlineData(Finger.RightThumb, Hand.Right)]
        public void GetHand(Finger finger, Hand expectedHand)
        {
            var hand = finger.GetHand();

            hand.ShouldBe(expectedHand);
        }

        [Theory]
        [MemberData(nameof(NeighborFingerList), parameters: true)]
        public void IsNeighbor_True(Finger first, Finger second)
        {
            var result = first.IsNeighbor(second);

            result.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(NeighborFingerList), parameters: false)]
        public void IsNeighbor_False(Finger first, Finger second)
        {
            var result = first.IsNeighbor(second);

            result.ShouldBeFalse();
        }

        public static IEnumerable<object[]> NeighborFingerList(bool isNeighbor)
        {
            var neighborsList = new HashSet<(Finger, Finger)>(new[]
            {
                (Finger.LeftLittle, Finger.LeftRing),
                (Finger.LeftRing, Finger.LeftMiddle),
                (Finger.LeftMiddle, Finger.LeftIndex),
                (Finger.RightLittle, Finger.RightRing),
                (Finger.RightRing, Finger.RightMiddle),
                (Finger.RightMiddle, Finger.RightIndex),
            }.SelectMany(x => new[] { (x.Item1, x.Item2), (x.Item2, x.Item1), }));

            if (isNeighbor)
            {
                foreach (var neighbors in neighborsList)
                {
                    yield return new object[] { neighbors.Item1, neighbors.Item2, };
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
                            yield return new object[] { neighbors.Item1, neighbors.Item2, };
                        }
                    }
                }
            }
        }
    }
}
