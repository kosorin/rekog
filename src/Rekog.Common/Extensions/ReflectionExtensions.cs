using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Rekog.Common.Extensions
{
    public static class ReflectionExtensions
    {
        public static object? GetDefaultValue(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsNullable())
            {
                return null;
            }

            object? defaultValue;

            var propertyType = propertyInfo.PropertyType;
            if (propertyType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(propertyType);
            }
            else if (propertyType == typeof(string))
            {
                defaultValue = string.Empty;
            }
            else
            {
                throw new NotSupportedException($"Type {propertyType.FullName} is not supported as a type for model form property.");
            }

            return defaultValue;
        }

        public static bool IsNullable(this PropertyInfo propertyInfo)
        {
            return IsNullableCore(propertyInfo.PropertyType, propertyInfo.DeclaringType, propertyInfo.CustomAttributes);
        }

        public static bool IsNullable(this FieldInfo fieldInfo)
        {
            return IsNullableCore(fieldInfo.FieldType, fieldInfo.DeclaringType, fieldInfo.CustomAttributes);
        }

        public static bool IsNullable(this ParameterInfo parameterInfo)
        {
            return IsNullableCore(parameterInfo.ParameterType, parameterInfo.Member, parameterInfo.CustomAttributes);
        }

        // https://stackoverflow.com/a/58454489/1933104
        private static bool IsNullableCore(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (memberType.IsValueType)
            {
                return Nullable.GetUnderlyingType(memberType) != null;
            }

            var nullable = customAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
            if (nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if (attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value! == 2;
                    }
                }
                else if (attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value! == 2;
                }
            }

            for (var type = declaringType; type != null; type = type.DeclaringType)
            {
                var context = type.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
                if (context != null && context.ConstructorArguments.Count == 1 && context.ConstructorArguments[0].ArgumentType == typeof(byte))
                {
                    return (byte)context.ConstructorArguments[0].Value! == 2;
                }
            }

            return false;
        }
    }
}
