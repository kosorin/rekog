using System.Reflection;

namespace Rekog.App.Undo.Batches
{
    public class PropertyInfoChangePropertyGroupKey : IChangePropertyGroupKey
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyInfoChangePropertyGroupKey(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public bool CanCoalesce(IChangePropertyGroupKey otherGroupKey)
        {
            if (ReferenceEquals(this, otherGroupKey))
            {
                return true;
            }

            return otherGroupKey is PropertyInfoChangePropertyGroupKey other
                && other._propertyInfo == _propertyInfo;
        }

        public bool AllowPropertyInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo == _propertyInfo;
        }
    }
}
