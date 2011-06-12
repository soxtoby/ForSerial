using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace json
{
    public class ObjectParser
    {
        private ParseValueFactory valueFactory;

        private ObjectParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseObject Parse(object obj, ParseValueFactory valueFactory)
        {
            ObjectParser parser = new ObjectParser(valueFactory);

            ParseValue value = parser.ParseValue(obj);

            return value.AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return valueFactory.CreateNull();

            switch (Type.GetTypeCode( input.GetType()))
            {
                case TypeCode.Object:
                    IEnumerable enumerable = input as IEnumerable;
                    if (enumerable != null)
                        return ParseArray(enumerable);
                    return ParseObject(input);

                case TypeCode.Boolean:
                    return valueFactory.CreateBoolean((bool)input);
                
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
                    return valueFactory.CreateNumber(Convert.ToDouble(input));

                case TypeCode.String:
                    return valueFactory.CreateString((string)input);

                default:
                    throw new ObjectParserException("Unknown TypeCode.", input);
            }
        }

        private ParseObject ParseObject(object obj)
        {
            if (!(obj is Object))
                throw new ObjectParserException("Expected Object.", obj);

            ParseObject output = valueFactory.CreateObject();

            Type type = obj.GetType();
            output.TypeIdentifier = GetTypeIdentifier(type);

            var properties = type.GetProperties()
                .Select(p => new {
                    Name = p.Name,
                    Get = p.GetGetMethod()
                })
                .Where(p => p.Get != null);

            foreach (var property in properties)
            {
                ParseValue value = ParseValue(property.Get.Invoke(obj, new object[] {  }));
                value.AddToObject(output, property.Name);
            }

            var fields = type.GetFields()
                .Where(f => f.IsPublic && !f.IsInitOnly && !f.IsNotSerialized);

            foreach (var field in fields)
            {
                ParseValue value = ParseValue(field.GetValue(obj));
                value.AddToObject(output, field.Name);
            }

            return output;
        }

        private ParseArray ParseArray(IEnumerable input)
        {
            ParseArray array = valueFactory.CreateArray();
            foreach (object item in input)
            {
                ParseValue value = ParseValue(item);
                value.AddToArray(array);
            }
            return array;
        }


        public static string GetTypeIdentifier(Type type)
        {
            return type.FullName;
        }


        public class ObjectParserException : Exception
        {
            public ObjectParserException(string message, object obj)
                : base(message + " Type: " + obj.GetType().FullName)
            { }
        }
    }
}

