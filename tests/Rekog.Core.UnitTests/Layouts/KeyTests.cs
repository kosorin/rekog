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
        public void GetRoll_True(Finger firstFinger, Finger secondFinger, Roll expectedRoll)
        {
            var firstKey = FingerToKey(firstFinger);
            var secondKey = FingerToKey(secondFinger);

            var roll = firstKey.GetHandRoll(secondKey);

            roll.ShouldBe(expectedRoll);
        }

        [Theory]
        [MemberData(nameof(RollFingerList), parameters: false)]
        public void GetRoll_False(Finger firstFinger, Finger secondFinger, Roll expectedRoll)
        {
            var firstKey = FingerToKey(firstFinger);
            var secondKey = FingerToKey(secondFinger);

            var roll = firstKey.GetHandRoll(secondKey);

            roll.ShouldBe(expectedRoll);
        }

        public static IEnumerable<object[]> RollFingerList(bool isRoll)
        {
            var rollList = new HashSet<(Finger, Finger, Roll)>(new[]
            {
                (Finger.LeftLittle, Finger.LeftRing, Roll.Inward),
                (Finger.LeftRing, Finger.LeftMiddle, Roll.Inward),
                (Finger.LeftMiddle, Finger.LeftIndex, Roll.Inward),
                (Finger.LeftIndex, Finger.LeftThumb, Roll.Inward),
                (Finger.LeftThumb, Finger.LeftIndex, Roll.Outward),
                (Finger.LeftIndex, Finger.LeftMiddle, Roll.Outward),
                (Finger.LeftMiddle, Finger.LeftRing, Roll.Outward),
                (Finger.LeftRing, Finger.LeftLittle, Roll.Outward),
                (Finger.RightLittle, Finger.RightRing, Roll.Inward),
                (Finger.RightRing, Finger.RightMiddle, Roll.Inward),
                (Finger.RightMiddle, Finger.RightIndex, Roll.Inward),
                (Finger.RightIndex, Finger.RightThumb, Roll.Inward),
                (Finger.RightThumb, Finger.RightIndex, Roll.Outward),
                (Finger.RightIndex, Finger.RightMiddle, Roll.Outward),
                (Finger.RightMiddle, Finger.RightRing, Roll.Outward),
                (Finger.RightRing, Finger.RightLittle, Roll.Outward),
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
                            yield return new object[] { first, second, Roll.None, };
                        }
                    }
                }
            }
        }
    }
}
