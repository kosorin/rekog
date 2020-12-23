using System.Collections.Generic;

namespace Rekog.Data
{
    public class Map<T> : List<List<T>>
    {
        public void Fix()
        {
            RemoveAll(x => x == null);
        }
    }
}
