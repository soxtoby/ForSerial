using System.Collections;
using json.Json;
using json.JsonObjects;
using NUnit.Framework;

namespace json.Tests.Json
{
    [TestFixture]
    public class JsonReaderTests
    {
        [Test]
        public void NoJson()
        {
            Assert.IsNull(ParseJson(string.Empty));
        }

        [Test]
        public void EmptyObject()
        {
            JsonObject obj = (JsonObject)ParseJson("{}");
            Assert.IsEmpty(obj);
        }

        [Test]
        public void NumberProperty()
        {
            Assert.AreEqual(5, ParseFooProperty<double>("{ \"foo\": 5 }"));
        }

        [Test]
        public void StringProperty()
        {
            Assert.AreEqual("bar", ParseFooProperty<string>("{ \"foo\": \"bar\" }"));
        }

        [Test]
        public void BooleanProperty()
        {
            Assert.IsTrue(ParseFooProperty<bool>("{ \"foo\": true }"));
            Assert.IsFalse(ParseFooProperty<bool>("{ \"foo\": false }"));
        }

        [Test]
        public void NullProperty()
        {
            Assert.IsNull(((JsonObject)ParseJson("{ \"foo\": null }"))["foo"]);
        }

        [Test]
        public void EmptyObjectProperty()
        {
            Assert.IsEmpty(ParseFooProperty<JsonObject>("{ \"foo\": { } }"));
        }

        [Test]
        public void ObjectThenNumberProperty()
        {
            JsonObject obj = (JsonObject)ParseJson("{ \"foo\": { }, \"bar\": 4 }");
            Assert.IsInstanceOf<JsonObject>(obj["foo"]);
            Assert.AreEqual(4, obj["bar"]);
        }

        [Test]
        public void NumberPropertyObjectProperty()
        {
            JsonObject objProperty = ParseFooProperty<JsonObject>("{ \"foo\": { \"bar\": 3 } }");
            Assert.AreEqual(3, objProperty["bar"]);
        }

        [Test]
        public void EmptyArrayProperty()
        {
            Assert.IsEmpty(ParseFooProperty<ICollection>("{ \"foo\": [ ] }"));
        }

        [Test]
        public void SingleNumberArrayProperty()
        {
            CollectionAssert.AreEqual(new[] { 1 }, ParseFooProperty<ICollection>("{ \"foo\": [1] }"));
        }

        [Test]
        public void MultipleNumberArrayProperty()
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ParseFooProperty<ICollection>("{ \"foo\": [ 1, 2, 3 ] }"));
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            CollectionAssert.AreEqual(new object[] { 1, "two", null }, ParseFooProperty<ICollection>("{ \"foo\": [ 1, \"two\", null ] }"));
        }

        [Test]
        public void ParseSubObject()
        {
            ParseSubObjectWriter writer = new ParseSubObjectWriter();
            Convert.From.Json(@"{""foo"":{""_type"":""bar"",""baz"":""qux""}}").WithBuilder(writer);

            Assert.AreEqual(@"{""baz"":""qux""}", writer.SubObjectJson);
        }

        private class ParseSubObjectWriter : TestWriter
        {
            public override OutputStructure BeginStructure()
            {
                return new ParseSubObjectObject(this);
            }

            public string SubObjectJson { get; set; }
        }

        private class ParseSubObjectObject : NullOutputStructure
        {
            private readonly ParseSubObjectWriter parentFactory;

            public ParseSubObjectObject(ParseSubObjectWriter parentFactory)
            {
                this.parentFactory = parentFactory;
            }

            public override bool SetType(string typeIdentifier, Reader reader)
            {
                parentFactory.SubObjectJson = JsonStringBuilder.GetResult(reader.ReadSubStructure(JsonStringBuilder.GetDefault()));
                return true;
            }
        }

        [Test]
        public void MaintainReferences()
        {
            var testBuilder = new WatchForReferenceBuilder();
            Convert.From.Json(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}").WithBuilder(testBuilder);

            Assert.NotNull(testBuilder.ReferencedObject);
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidName_ThrowsParseException()
        {
            ParseJson("{ foo: 0 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void NoColon_ThrowsParseException()
        {
            ParseJson("{ \"foo\" 0 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidWordValue_ThrowsParseException()
        {
            ParseJson("{ \"foo\": bar }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidSymbolValue_ThrowsParseException()
        {
            ParseJson("{ \"foo\": ] }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void MissingClosingBrace_ThrowsParseException()
        {
            ParseJson("{ \"foo\": { }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void TrailingCommaInObject_ThrowsParseException()
        {
            ParseJson("{ \"foo\": 5, }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void MissingClosingBracket_ThrowsParseException()
        {
            ParseJson("{ \"foo\": [ 5 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void TrailingCommaInArray_ThrowsParseException()
        {
            ParseJson("{ \"foo\": [ 5, ] }");
        }

        private static T ParseFooProperty<T>(string json)
        {
            JsonObject obj = (JsonObject)ParseJson(json);
            Assert.IsInstanceOf<T>(obj["foo"]);
            return (T)obj["foo"];
        }

        private static object ParseJson(string json)
        {
            return Convert.From.Json(json).ToJsonObject();
        }

        [Test]
        public void CreatePropertyObject()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.Json(@"{""foo"":{}}").WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        }

        [Test]
        public void CreatePropertyArray()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.Json(@"{""foo"":[]}").WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        }

        [Test]
        public void CreateArrayObject()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.Json(@"{""foo"":[{}]}").WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        }

        [Test]
        public void CreateArrayArray()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.Json(@"{""foo"":[[]]}").WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        }
    }
}
