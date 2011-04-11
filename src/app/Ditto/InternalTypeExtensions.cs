using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto
{
    internal static class InternalTypeExtensions
    {
        private static List<byte[]> tokens = new List<byte[]>()
                                                 {
                                                     new byte[] {0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89},
                                                     new byte[] {0x31, 0xbf, 0x38, 0x56, 0xad, 0x36, 0x4e, 0x35},
                                                     new byte[] {0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a}
                                                 };
        public class ByteArrayEqualityComparer : EqualityComparer<byte[]>
        {
            public override bool Equals(byte[] x, byte[] y)
            {
                return x != null && y != null
                            && x.Length == 8 && y.Length == 8
                            && x[0] == y[0]
                            && x[1] == y[1]
                            && x[2] == y[2]
                            && x[3] == y[3]
                            && x[4] == y[4]
                            && x[5] == y[5]
                            && x[6] == y[6]
                            && x[7] == y[7];
            }

            public override int GetHashCode(byte[] obj)
            {
                return obj.GetHashCode();
            }
        }
        /// <summary>
        /// Shamelessly stolen from http://stackoverflow.com/questions/3642806/how-to-determine-and-check-whether-a-type-in-assembly-is-custom-type-or-primitive
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is framework type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFrameworkType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();

            return publicKeyToken != null && publicKeyToken.Length == 8
                   && tokens.Contains(publicKeyToken, new ByteArrayEqualityComparer());
        }

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