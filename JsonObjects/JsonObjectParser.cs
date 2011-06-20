using System;
using System.Collections;

namespace json.JsonObjects
{
    public class JsonObjectParser : Parser
    {
        private const string TypeKey = "_type";
        private readonly ParseValueFactory valueFactory;

        private JsonObjectParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseObject Parse(JsonObject obj, ParseValueFactory valueFactory)
        {
            JsonObjectParser parser = new JsonObjectParser(valueFactory);
            return parser.ParseObject(obj);
        }

        public ParseObject ParseSubObject(ParseValueFactory valueFactory)
        {
            throw new NotImplementedException();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return valueFactory.CreateNull();

            switch (Type.GetTypeCode(input.GetType()))
            {
                case TypeCode.Object:
                    JsonObject jsonObject = input as JsonObject;
                    IEnumerable enumerable;
                    if (jsonObject != null)
                        return ParseObject(jsonObject);
                    if ((enumerable = input as IEnumerable) != null)
                        return ParseArray(enumerable);

                    throw new InvalidObject(input.GetType());

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

        private ParseObject ParseObject(JsonObject obj)
        {
            ParseObject parseObject = valueFactory.CreateObject();

            foreach (var property in obj)
            {
                if (property.Key == TypeKey)
                    parseObject.SetType((string)obj[TypeKey], this);
                else
                    ParseValue(property.Value).AddToObject(parseObject, property.Key);
            }

            return parseObject;
        }

        private ParseArray ParseArray(IEnumerable enumerable)
        {
            ParseArray array = valueFactory.CreateArray();

            foreach (object item in enumerable)
                ParseValue(item).AddToArray(array);

            return array;
        }

        internal class InvalidObject : Exception
        {
            public InvalidObject(Type objectType) : base("Cannot parse object of type {0}.".FormatWith(objectType.FullName)) { }
        }

        internal class UnknownTypeCode : Exception
        {
            public UnknownTypeCode(object obj)
                : base("Type {0} has unknown TypeCode.".FormatWith(obj.GetType().FullName))
            { }
        }
    }
}
