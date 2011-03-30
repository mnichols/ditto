using System;
using System.Linq;

namespace Ditto
{
    internal static class InternalTypeExtensions
    {
        internal static T As<T>(this object obj)
        {
            return (T) obj;
        }

        internal static bool IsOneOf(this Type type, params Type[] these)
        {
            return these.Any(are => are == type);
        }

        internal static bool IsNullableType(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)));
        }
        internal static object DefaultValue(this Type type)
        {
            if (type.IsValueType == false)
                return null;

            return Activator.CreateInstance(type);
            
        }
        internal static bool IsDefaultOrEmpty(this Type type,object value)
        {
            if (value == null)
                return true;
            if (type == typeof(string))
                return string.IsNullOrEmpty((string)value);
            return Equals(value,type.DefaultValue());
        }
        internal static Type ElementType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            if (type.IsGenericType)
                return type.GetGenericArguments()[0];
            throw new NotSupportedException("Determining element type of "+type+" not supported.");
        }
    }
}