using System;

namespace json.Objects.TypeDefinitions
{
    internal class PrimitiveDefinition : TypeDefinition
    {
        private readonly Action<object, Writer> typedRead;

        private PrimitiveDefinition(Type type)
            : base(type)
        {
            typedRead = GetWriterMethod(TypeCode);
        }

        internal static Action<object, Writer> GetWriterMethod(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return (o, writer) => writer.Write((bool)o);
                case TypeCode.Int16:
                    return (o, writer) => writer.Write((Int16)o);
                case TypeCode.Int32:
                    return (o, writer) => writer.Write((Int32)o);
                case TypeCode.Int64:
                    return (o, writer) => writer.Write((Int64)o);
                case TypeCode.Single:
                    return (o, writer) => writer.Write((Single)o);
                case TypeCode.Double:
                    return (o, writer) => writer.Write((Double)o);
                case TypeCode.Decimal:
                    return (o, writer) => writer.Write((Decimal)o);
                case TypeCode.String:
                    return (o, writer) => writer.Write((String)o);
                case TypeCode.Char:
                    return (o, writer) => writer.Write((Char)o);
                case TypeCode.SByte:
                    return (o, writer) => writer.Write((SByte)o);
                case TypeCode.Byte:
                    return (o, writer) => writer.Write((Byte)o);
                case TypeCode.UInt16:
                    return (o, writer) => writer.Write((UInt16)o);
                case TypeCode.UInt32:
                    return (o, writer) => writer.Write((UInt32)o);
                case TypeCode.UInt64:
                    return (o, writer) => writer.Write((UInt64)o);
            }
            return (o, writer) => { };
        }

        internal static PrimitiveDefinition CreatePrimitiveTypeDefinition(Type type)
        {
            return IsPrimitiveType(type)
                ? new PrimitiveDefinition(type)
                : null;
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string);  // we'll consider string to be a primitive for purposes of JSON serialization
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            typedRead(input, writer);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(ConvertToCorrectType(value));
        }
    }
}