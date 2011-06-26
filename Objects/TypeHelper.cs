using System;
using System.Linq;

namespace json.Objects
{
    internal static class TypeHelper
    {
        public static Type GetGenericInterfaceType(this Type derivedType, Type genericType, int typeIndex = 0)
        {
            return derivedType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType)
                .Select(i => i.GetGenericArguments().ElementAtOrDefault(typeIndex)).FirstOrDefault();
        }

        public static TypeCodeType GetTypeCodeType(this Type type)
        {
            switch (Type.GetTypeCode(type))
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