using System;
using System.Collections;
using System.Collections.Generic;
using json.Objects;

namespace json.JsonObjects
{
    public class JsonObjectParser : Parser
    {
        private const string TypeKey = "_type";
        private JsonObject currentObject;
        private readonly Dictionary<JsonObject, ParseObject> objectReferences = new Dictionary<JsonObject, ParseObject>();

        private JsonObjectParser(ParseValueFactory valueFactory) : base(valueFactory) { }

        public static ParseObject Parse(JsonObject obj, ParseValueFactory valueFactory)
        {
            JsonObjectParser parser = new JsonObjectParser(valueFactory);
            return parser.ParseObject(obj);
        }

        public override ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            return Parse(currentObject, subParseValueFactory);
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return valueFactory.Current.CreateValue(null);

            switch (input.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    JsonObject jsonObject = input as JsonObject;
                    IEnumerable enumerable;
                    if (jsonObject != null)
                        return ParseObject(jsonObject);
                    if ((enumerable = input as IEnumerable) != null)
                        return ParseArray(enumerable);

                    throw new InvalidObject(input.GetType());

                case TypeCodeType.Boolean:
                case TypeCodeType.String:
                case TypeCodeType.Number:
                    return valueFactory.Current.CreateValue(input);

                default:
                    throw new UnknownTypeCode(input);
            }
        }

        private ParseObject ParseObject(JsonObject obj)
        {
            ParseObject existingReference = objectReferences.Get(obj);
            return existingReference == null
                ? ParseNewObject(obj)
                : ReferenceObject(existingReference);
        }

        private ParseObject ParseNewObject(JsonObject obj)
        {
            currentObject = obj;

            ParseObject parseObject = objectReferences[obj] = valueFactory.Current.CreateObject();

            foreach (var property in obj)
            {
                string name = property.Key;
                object value = property.Value;
                using (UseObjectPropertyContext(parseObject, name))
                {
                    if (name == TypeKey)
                        parseObject.SetType((string)obj[TypeKey], this);
                    else
                        ParseValue(value).AddToObject(parseObject, name);
                }
            }

            return parseObject;
        }

        private ParseObject ReferenceObject(ParseObject existingReference)
        {
            return valueFactory.Current.CreateReference(existingReference);
        }

        private ParseArray ParseArray(IEnumerable enumerable)
        {
            ParseArray array = valueFactory.Current.CreateArray();

            using (UseArrayContext(array))
            {
                foreach (object item in enumerable)
                    ParseValue(item).AddToArray(array);
            }

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
