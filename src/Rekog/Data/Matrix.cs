using System.Collections.Generic;

namespace Rekog.Data
{
    public class Matrix<T> : List<List<T>>
    {
        public void Fix()
        {
            RemoveAll(x => x == null);
        }
    }
}
