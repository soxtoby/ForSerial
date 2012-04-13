using System;
using System.Linq;
using ForSerial.Json;
using ForSerial.JsonObjects;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.Json
{
    [TestFixture]
    public class JsonReaderTests
    {
        [Test]
        public void NoJson()
        {
            ParseJson(string.Empty).ShouldBeNull();
        }

        [Test]
        public void WhitespaceOnly()
        {
            ParseJson(" ").ShouldBeNull();
        }

        [Test]
        public void Null()
        {
            ParseJson("null")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBeNull();
        }

        [Test]
        public void True()
        {
            ParseJson("true")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(true);
        }

        [Test]
        public void False()
        {
            ParseJson("false")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(false);
        }

        [Test]
        public void Number()
        {
            ParseJson("1")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(1);
        }

        [Test]
        public void NegativeNumber()
        {
            ParseJson("-1")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(-1);
        }

        [Test]
        public void String()
        {
            ParseJson(@"""foo""")
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe("foo");
        }

        [Test]
        public void EmptyMap()
        {
            ParseJson("{}")
                .ShouldBe<JsonMap>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void EmptyArray()
        {
            ParseJson("[]")
                .ShouldBe<JsonArray>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void NullProperty()
        {
            ParseFooProperty(@"{ ""foo"": null }")
                .Value().ShouldBeNull();
        }

        [Test]
        public void BooleanProperty()
        {
            ParseFooProperty(@"{ ""foo"": true }")
                .Value().ShouldBe(true);
        }

        [Test]
        public void NumberProperty()
        {
            ParseFooProperty(@"{ ""foo"": 5 }")
                .Value().ShouldBe(5);
        }

        [Test]
        public void NegativeNumberProperty()
        {
            ParseFooProperty(@"{ ""foo"": -5 }")
                .Value().ShouldBe(-5);
        }

        [Test]
        public void StringProperty()
        {
            ParseFooProperty(@"{ ""foo"": ""bar"" }")
                .Value().ShouldBe("bar");
        }

        [Test]
        public void EmptyStringProperty()
        {
            ParseFooProperty(@"{ ""foo"": """" }")
                .Value().ShouldBe("");
        }

        [Test]
        public void EmptyMapProperty()
        {
            ParseFooProperty(@"{ ""foo"": { } }")
                .ShouldBe<JsonMap>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void ObjectThenNumberProperty()
        {
            ParseJson(@"{ ""foo"": { }, ""bar"": 4 }")
                .ShouldBe<JsonMap>()
                .And(map => map["foo"].ShouldBe<JsonMap>())
                .And(map => map["bar"].Value().ShouldBe(4));
        }

        [Test]
        public void NumberPropertyObjectProperty()
        {
            ParseFooProperty(@"{ ""foo"": { ""bar"": 3 } }")
                .ShouldBe<JsonMap>()
                .And.Value("bar").ShouldBe(3);
        }

        [Test]
        public void EmptyArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ ] }")
                .ShouldBe<JsonArray>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void SingleNumberArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [1] }")
                .ShouldBe<JsonArray>()
                .And.Single().Value().ShouldBe(1);
        }

        [Test]
        public void MultipleNumberArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ 1, 2, 3 ] }")
                .ShouldBe<JsonArray>()
                .And.Values().ShouldMatch<int>(new[] { 1, 2, 3 });
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ 1, ""two"", null ] }")
                .ShouldBe<JsonArray>()
                .And.Values().ShouldMatch(new object[] { 1, "two", null });
        }

        [Test]
        public void MaintainReferences()
        {
            Writer writer = Substitute.For<Writer>();

            JsonReader.Read(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", writer);

            writer.Received().WriteReference(1);
        }

        [Test]
        public void ReferenceNotCountedAsStructure()
        {
            Writer writer = Substitute.For<Writer>();

            JsonReader.Read(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", writer);

            writer.Received(2).BeginStructure(Arg.Any<Type>());
            writer.Received(2).EndStructure();
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

        private static JsonObject ParseFooProperty(string json)
        {
            JsonObject result = ParseJson(json);
            result.ShouldBe<JsonMap>();
            return result.Get("foo");
        }

        private static JsonObject ParseJson(string json)
        {
            JsonObjectWriter writer = new JsonObjectWriter();
            JsonReader.Read(json, writer);
            return writer.Result;
        }
    }
}
