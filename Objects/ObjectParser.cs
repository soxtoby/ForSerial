using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectParser : Parser
    {
        private readonly ParseValueFactory valueFactory;
        private readonly Options options;
        private object currentObject;

        private ObjectParser(ParseValueFactory valueFactory, Options options)
        {
            this.valueFactory = valueFactory;
            this.options = options;
        }

        public static ParseValue Parse(object obj, ParseValueFactory valueFactory, Options options = Options.Default)
        {
            ObjectParser parser = new ObjectParser(valueFactory, options);

            return parser.ParseValue(obj);
        }

        public ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            return Parse(currentObject, subParseValueFactory).AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return valueFactory.CreateNull();

            switch (Type.GetTypeCode(input.GetType()))
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
                    throw new UnknownTypeCode(input);
            }
        }

        private ParseObject ParseObject(object obj)
        {
            ParseObject output = valueFactory.CreateObject();

            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(obj.GetType());

            currentObject = obj;
            output.SetType(typeDef.Type.AssemblyQualifiedName, this);

            IEnumerable<PropertyDefinition> propertiesToSerialize = SerializeOneWayTypes
                ? typeDef.Properties.Values
                : typeDef.SerializableProperties;

            foreach (PropertyDefinition property in propertiesToSerialize)
            {
                ParseValue value = ParseValue(property.GetFrom(obj));
                value.AddToObject(output, property.Name);
            }

            return output;
        }

        private bool SerializeOneWayTypes
        {
            get { return (options & Options.SerializeOneWayTypes) != 0; }
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

        public class UnknownTypeCode : Exception
        {
            public UnknownTypeCode(object obj)
                : base("Type {0} has unknown TypeCode.".FormatWith(obj.GetType().FullName))
            { }
        }

        [Flags]
        public enum Options
        {
            Default = 0,

            /// <summary>
            /// Allows serialization of types that cannot be deserialized.
            /// </summary>
            SerializeOneWayTypes = 1,
        }
    }
}

