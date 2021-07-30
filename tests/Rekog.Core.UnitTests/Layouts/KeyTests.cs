using System;
using System.Collections.Generic;
using System.Linq;
using Rekog.Core.Layouts;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Layouts
{
    public class KeyTests
    {
        private static Key FingerToKey(Finger finger)
        {
            return new Key(default, finger, default, default, default, default, default);
        }

        [Theory]
        [MemberData(nameof(RollFingerList), parameters: true)]
        public void GetRoll_True(Finger firstFinger, Finger secondFinger, Direction expectedRoll)
        {
            var firstKey = FingerToKey(firstFinger);
            var secondKey = FingerToKey(secondFinger);

            var roll = firstKey.GetHandRoll(secondKey);

            roll.ShouldBe(expectedRoll);
        }

        [Theory]
        [MemberData(nameof(RollFingerList), parameters: false)]
        public void GetRoll_False(Finger firstFinger, Finger secondFinger, Direction expectedRoll)
        {
            var firstKey = FingerToKey(firstFinger);
            var secondKey = FingerToKey(secondFinger);

            var roll = firstKey.GetHandRoll(secondKey);

            roll.ShouldBe(expectedRoll);
        }

        public static IEnumerable<object[]> RollFingerList(bool isRoll)
        {
            var rollList = new HashSet<(Finger, Finger, Direction)>(new[]
            {
                (Finger.LeftLittle, Finger.LeftRing, Direction.Inward),
                (Finger.LeftRing, Finger.LeftMiddle, Direction.Inward),
                (Finger.LeftMiddle, Finger.LeftIndex, Direction.Inward),
                (Finger.LeftIndex, Finger.LeftThumb, Direction.Inward),
                (Finger.LeftThumb, Finger.LeftIndex, Direction.Outward),
                (Finger.LeftIndex, Finger.LeftMiddle, Direction.Outward),
                (Finger.LeftMiddle, Finger.LeftRing, Direction.Outward),
                (Finger.LeftRing, Finger.LeftLittle, Direction.Outward),
                (Finger.RightLittle, Finger.RightRing, Direction.Inward),
                (Finger.RightRing, Finger.RightMiddle, Direction.Inward),
                (Finger.RightMiddle, Finger.RightIndex, Direction.Inward),
                (Finger.RightIndex, Finger.RightThumb, Direction.Inward),
                (Finger.RightThumb, Finger.RightIndex, Direction.Outward),
                (Finger.RightIndex, Finger.RightMiddle, Direction.Outward),
                (Finger.RightMiddle, Finger.RightRing, Direction.Outward),
                (Finger.RightRing, Finger.RightLittle, Direction.Outward),
            });

            if (isRoll)
            {
                foreach (var roll in rollList)
                {
                    yield return new object[] { roll.Item1, roll.Item2, roll.Item3, };
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
                            yield return new object[] { first, second, Direction.None, };
                        }
                    }
                }
            }
        }
    }
}
