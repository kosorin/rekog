using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue? GetSameOrDefaultValue<TItem, TValue>(this ICollection<TItem> items, Func<TItem, TValue> selector, TValue? defaultValue = default)
        {
            if (items.Count == 0)
            {
                return defaultValue;
            }

            var value = selector(items.First());

            if (items.Count == 1 || items.Skip(1).All(x => Equals(selector(x), value)))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
