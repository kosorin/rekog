using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rekog.App.Reflection
{
    public static class ReflectionCache
    {
        private static readonly Dictionary<(Type type, string propertyName), PropertyInfo> PropertyInfos = new Dictionary<(Type type, string propertyName), PropertyInfo>();

        public static PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            return GetPropertyInfo(instance.GetType(), propertyName);
        }

        public static PropertyInfo GetPropertyInfo<T>(string propertyName)
        {
            return GetPropertyInfo(typeof(T), propertyName);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            var key = (type, propertyName);

            if (PropertyInfos.TryGetValue(key, out var propertyInfo))
            {
                return propertyInfo;
            }

            propertyInfo = type.GetProperty(propertyName);

            PropertyInfos.Add(key, propertyInfo ?? throw new ArgumentException(null, nameof(propertyName)));

            return propertyInfo;
        }
    }
}
