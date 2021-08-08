using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
        {
            return enumerable.Where(x => x != null).Select(x => x!);
        }
    }
}
