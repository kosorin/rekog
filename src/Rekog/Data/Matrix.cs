using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public class Matrix<T> : List<List<T>>
    {
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void Fix()
        {
            RemoveAll(x => x == null);
        }
    }
}
