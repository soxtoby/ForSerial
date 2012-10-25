using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForSerial.Objects
{
    internal static class ReflectionHelper
    {
        private const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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

            Type valueType = value.GetType();
            return valueType == typeof(bool)
                || valueType == typeof(string)
                || valueType == typeof(char)
                || valueType == typeof(byte)
                || valueType == typeof(Int16)
                || valueType == typeof(Int32)
                || valueType == typeof(Int64)
                || valueType == typeof(UInt16)
                || valueType == typeof(UInt32)
                || valueType == typeof(UInt64)
                || valueType == typeof(Single)
                || valueType == typeof(double)
                || valueType == typeof(decimal);
        }

        internal static bool IsAutoPropertyBackingField(this string name)
        {
            return name.StartsWith("<") && name.Contains("BackingField");
        }

        internal static MemberInfo GetSourceMember(this FieldInfo field)
        {
            return IsAutoPropertyBackingField(field.Name)
                ? (MemberInfo)GetAutoPropertyInfo(field)
                : field;
        }

        private static PropertyInfo GetAutoPropertyInfo(FieldInfo field)
        {
            return field.DeclaringType.GetProperty(AutoPropertyName(field.Name), InstanceMembers);
        }

        internal static string AutoPropertyName(this string name)
        {
            return name.Substring(1, name.IndexOf('>') - 1);
        }

        internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            PropertyInfo[] properties = type.GetProperties(InstanceMembers);
            return type.BaseType == null
                ? properties
                : properties.Concat(type.BaseType.GetAllProperties());
        }

        internal static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            FieldInfo[] fields = type.GetFields(InstanceMembers);
            return type.BaseType == null
                ? fields
                : fields.Concat(type.BaseType.GetAllFields());
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
