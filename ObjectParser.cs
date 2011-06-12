using System;
using System.Collections;

namespace json
{
    public class ObjectParser
    {
        private ParseValueFactory valueFactory;

        private ObjectParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseValue Parse(object obj, ParseValueFactory valueFactory)
        {
            ObjectParser parser = new ObjectParser(valueFactory);

            return parser.ParseValue(obj);
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

            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(obj.GetType());

            output.SetTypeIdentifier(typeDef.Type.AssemblyQualifiedName);

            foreach (PropertyDefinition property in typeDef.Properties.Values)
            {
                ParseValue value = ParseValue(property.GetFrom(obj));
                value.AddToObject(output, property.Name);
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

