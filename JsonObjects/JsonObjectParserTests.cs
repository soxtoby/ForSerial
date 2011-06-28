using System;
using json.Json;
using NUnit.Framework;

namespace json.JsonObjects
{
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

        [Test]
        public void ParseSubObject()
        {
            JsonObject obj = new JsonObject { { "foo", new JsonObject { { "_type", "bar" }, { "baz", "qux" } } } };
            ParseSubObjectValueFactory valueFactory = new ParseSubObjectValueFactory();
            Parse.From.JsonObject(obj).WithBuilder(valueFactory);

            Assert.AreEqual(@"{""baz"":""qux""}", valueFactory.SubObjectJson);
        }

        private class ParseSubObjectValueFactory : TestValueFactory
        {
            public string SubObjectJson { get; set; }

            public override ParseObject CreateObject()
            {
                return new ParseSubObjectObject(this);
            }
        }

        private class ParseSubObjectObject : TestParseObject
        {
            private readonly ParseSubObjectValueFactory parentFactory;

            public ParseSubObjectObject(ParseSubObjectValueFactory parentFactory)
            {
                this.parentFactory = parentFactory;
            }

            public override bool SetType(string typeIdentifier, Parser parser)
            {
                ParseObject jsonStringObject = parser.ParseSubObject(JsonStringBuilder.Default);
                parentFactory.SubObjectJson = JsonStringBuilder.GetResult(jsonStringObject);
                return true;
            }
        }


        [Test]
        [ExpectedException(typeof(JsonObjectParser.InvalidObject))]
        public void InvalidObject()
        {
            ParseJsonObject(new JsonObject { { "foo", new { } } });
        }

        [Test]
        [ExpectedException(typeof(JsonObjectParser.UnknownTypeCode))]
        public void UnknownTypeCode()
        {
            ParseJsonObject(new JsonObject { { "foo", DBNull.Value } });
        }

        private static string ParseJsonObject(JsonObject jsonObject)
        {
            return Parse.From.JsonObject(jsonObject).ToJson();
        }
    }
}