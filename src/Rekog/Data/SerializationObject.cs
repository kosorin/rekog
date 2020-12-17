using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Data
{
    public abstract record SerializationObject
    {
        public void FixAll()
        {
            FixSelf();

            foreach (var child in CollectChildren())
            {
                child.FixAll();
            }
        }

        protected abstract void FixSelf();

        protected Dictionary<string, T> FixMap<T>(Dictionary<string, T> map)
        {
            return FixMap(map, x => x == null);
        }

        protected Dictionary<string, T> FixMap<T>(Dictionary<string, T> map, Predicate<T?> removePredicate)
        {
            map ??= new();

            var toRemove = map.Where(x => removePredicate.Invoke(x.Value)).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
            {
                map.Remove(key);
            }

            return map;
        }

        protected abstract IEnumerable<SerializationObject> CollectChildren();
    }
}
