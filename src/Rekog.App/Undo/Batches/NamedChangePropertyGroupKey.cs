using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rekog.App.Undo.Batches
{
    public class NamedChangePropertyGroupKey : IChangePropertyGroupKey
    {
        private readonly string _name;
        private readonly IList<PropertyInfo> _propertyInfos;

        public NamedChangePropertyGroupKey(string name, IEnumerable<PropertyInfo> propertyInfos)
        {
            _name = name;
            _propertyInfos = propertyInfos.ToList();
        }

        public bool CanCoalesce(IChangePropertyGroupKey otherGroupKey)
        {
            if (ReferenceEquals(this, otherGroupKey))
            {
                return true;
            }

            return otherGroupKey is NamedChangePropertyGroupKey other
                && other._name == _name
                && other._propertyInfos.SequenceEqual(_propertyInfos);
        }

        public bool AllowPropertyInfo(PropertyInfo propertyInfo)
        {
            return _propertyInfos.Contains(propertyInfo);
        }
    }
}
