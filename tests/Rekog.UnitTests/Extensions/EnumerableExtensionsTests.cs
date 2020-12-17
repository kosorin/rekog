﻿using Rekog.Extensions;
using Shouldly;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void DistinctBy()
        {
            var items = new DistinctByTestData[] { 1, 1, 1, 2, 3, 3, 4, 4, 4 };
            var expectedItems = new DistinctByTestData[] { 1, 2, 3, 4 };

            var resultItems = EnumerableExtensions.DistinctBy(items, x => x.Value).ToArray();

            resultItems.ShouldBe(expectedItems, ignoreOrder: true);
        }

        private record DistinctByTestData(int Value)
        {
            public static implicit operator DistinctByTestData(int value)
            {
                return new(value);
            }
        }
    }
}
