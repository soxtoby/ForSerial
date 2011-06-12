using System.Collections;
using NUnit.Framework;

namespace json
{
    [TestFixture]
    public class JsonParserTests
    {
        [Test]
        public void NoJson()
        {
            Assert.IsNull(Parse(string.Empty));
        }

        [Test]
        public void EmptyObject()
        {
            JsonDictionary obj = Parse("{}");
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
            Assert.IsNull(Parse("{ \"foo\": null }")["foo"]);
        }

        [Test]
        public void EmptyObjectProperty()
        {
            Assert.IsEmpty(ParseFooProperty<JsonDictionary>("{ \"foo\": { } }"));
        }

        [Test]
        public void ObjectThenNumberProperty()
        {
            JsonDictionary obj = Parse("{ \"foo\": { }, \"bar\": 4 }");
            Assert.IsInstanceOfType(typeof(JsonDictionary), obj["foo"]);
            Assert.AreEqual(4, obj["bar"]);
        }

        [Test]
        public void NumberPropertyObjectProperty()
        {
            JsonDictionary objProperty = ParseFooProperty<JsonDictionary>("{ \"foo\": { \"bar\": 3 } }");
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
        [ExpectedException(typeof(ParseException))]
        public void NoOpenBrace_ThrowsParseException()
        {
            Parse("5");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidName_ThrowsParseException()
        {
            Parse("{ foo: 0 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void NoColon_ThrowsParseException()
        {
            Parse("{ \"foo\" 0 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidWordValue_ThrowsParseException()
        {
            Parse("{ \"foo\": bar }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void InvalidSymbolValue_ThrowsParseException()
        {
            Parse("{ \"foo\": ] }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void MissingClosingBrace_ThrowsParseException()
        {
            Parse("{ \"foo\": { }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void TrailingCommaInObject_ThrowsParseException()
        {
            Parse("{ \"foo\": 5, }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void MissingClosingBracket_ThrowsParseException()
        {
            Parse("{ \"foo\": [ 5 }");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void TrailingCommaInArray_ThrowsParseException()
        {
            Parse("{ \"foo\": [ 5, ] }");
        }


        private T ParseFooProperty<T>(string json)
        {
            JsonDictionary obj = Parse(json);
            Assert.IsInstanceOfType(typeof(T), obj["foo"]);
            return (T)obj["foo"];
        }

        private JsonDictionary Parse(string json)
        {
            return JsonParser.Parse(Scanner.Scan(json));
        }
    }
}
