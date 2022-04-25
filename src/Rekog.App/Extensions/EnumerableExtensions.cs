using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
        {
            return enumerable.Where(x => x != null).Select(x => x!);
        }
        
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct
        {
            return enumerable.Where(x => x.HasValue).Select(x => x!.Value);
        }
    }
}
