using System;
using System.Collections;
using NUnit.Framework;

namespace json.JsonObjects
{
    public class JsonObjectParser : Parser
    {
        private const string typeKey = "_type";
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
                if (property.Key == typeKey)
                    parseObject.SetType((string)obj[typeKey], this);
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

    [TestFixture]
    public class JsonObjectParserTests
    {
        [Test]
        public void EmptyObject()
        {
            Assert.AreEqual("{}", ParseJsonObject(new JsonObject()));
        }

        [Test]
        public void NullProperty()
        {
            Assert.AreEqual(@"{""foo"":null}", ParseJsonObject(new JsonObject { { "foo", null } }));
        }

        [Test]
        public void BooleanProperty()
        {
            Assert.AreEqual(@"{""foo"":true}", ParseJsonObject(new JsonObject { { "foo", true } }));
            Assert.AreEqual(@"{""foo"":false}", ParseJsonObject(new JsonObject { { "foo", false } }));
        }

        [Test]
        public void StringProperty()
        {
            Assert.AreEqual(@"{""foo"":""bar""}", ParseJsonObject(new JsonObject { { "foo", "bar" } }));
        }

        [Test]
        public void NumberProperty()
        {
            Assert.AreEqual(@"{""foo"":5}", ParseJsonObject(new JsonObject { { "foo", 5 } }));
        }

        [Test]
        public void EmptyObjectProperty()
        {
            Assert.AreEqual(@"{""foo"":{}}", ParseJsonObject(new JsonObject { { "foo", new JsonObject() } }));
        }

        [Test]
        public void NonEmptyObjectProperty()
        {
            Assert.AreEqual(@"{""foo"":{""bar"":5}}", ParseJsonObject(new JsonObject { { "foo", new JsonObject { { "bar", 5 } } } }));
        }

        [Test]
        public void NumberArrayProperty()
        {
            Assert.AreEqual(@"{""foo"":[1,2,3]}", ParseJsonObject(new JsonObject { { "foo", new[] { 1, 2, 3 } } }));
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            Assert.AreEqual(@"{""foo"":[1,""two"",{""three"":4},[5]]}", ParseJsonObject(
                new JsonObject
                {
                    { "foo", new object[]
                        {
                            1,
                            "two",
                            new JsonObject { {"three", 4 }},
                            new[] { 5 },
                        }}
                }));
        }

        private static string ParseJsonObject(JsonObject jsonObject)
        {
            return Parse.From.JsonObject(jsonObject).ToJson();
        }
    }
}
