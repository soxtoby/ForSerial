using System;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    internal static class ReflectionHelper
    {
        public static Type GetGenericInterfaceType(this Type derivedType, Type genericType, int typeIndex = 0)
        {
            return derivedType.GetInterfaces()
                .Append(derivedType)
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType)
                .Select(i => i.GetGenericArguments().ElementAtOrDefault(typeIndex)).FirstOrDefault();
        }

        public static TypeCodeType GetTypeCodeType(this Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            return GetTypeCodeType(typeCode);
        }

        public static TypeCodeType GetTypeCodeType(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object:
                    return TypeCodeType.Object;
                case TypeCode.Boolean:
                    return TypeCodeType.Boolean;
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return TypeCodeType.Number;
                case TypeCode.String:
                    return TypeCodeType.String;
                default:
                    return TypeCodeType.Other;
            }
        }

        /// <summary>
        /// Because IsAssignableFrom confuses the bajeezus outa me.
        /// </summary>
        public static bool CanBeCastTo(this Type type, Type castTo)
        {
            return castTo.IsAssignableFrom(type);
        }

        public static bool HasAttribute<T>(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool IsJsonPrimitiveType(this object value)
        {
            if (value == null)
                return true;

            TypeCodeType typeCodeType = value.GetType().GetTypeCodeType();
            return typeCodeType == TypeCodeType.Number
                || typeCodeType == TypeCodeType.Boolean
                || typeCodeType == TypeCodeType.String;
        }
    }

    internal enum TypeCodeType
    {
        Object,
        Boolean,
        String,
        Number,
        Other
    }
}