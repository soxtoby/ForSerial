using System;
using System.Linq;
using EasyAssertions;
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
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBeNull();
        }

        [Test]
        public void True()
        {
            ParseJson("true")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(true);
        }

        [Test]
        public void False()
        {
            ParseJson("false")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(false);
        }

        [Test]
        public void Number()
        {
            ParseJson("1")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(1d, 0);
        }

        [Test]
        public void NegativeNumber()
        {
            ParseJson("-1")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(-1d, 0);
        }

        [Test]
        public void String()
        {
            ParseJson(@"""foo""")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo");
        }

        [Test]
        public void StringWithEscapedBackslash_IsUnescaped()
        {
            ParseJson(@"""foo\\bar""")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo\\bar");
        }

        [Test]
        public void StringWithEscapedQuotes_IsUnescaped()
        {
            ParseJson(@"""foo\""bar""")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo\"bar");
        }

        [Test]
        public void StringWithEscapedReturn_IsUnescaped()
        {
            ParseJson(@"""foo\rbar""")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo\rbar");
        }

        [Test]
        public void StringWithEscapedNewLine_IsUnescaped()
        {
            ParseJson(@"""foo\nbar""")
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo\nbar");
        }

        [Test]
        public void EmptyMap()
        {
            ParseJson("{}")
                .ShouldBeA<JsonMap>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void EmptyArray()
        {
            ParseJson("[]")
                .ShouldBeA<JsonArray>()
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
                .Value().ShouldBe(5d, 0);
        }

        [Test]
        public void NegativeNumberProperty()
        {
            ParseFooProperty(@"{ ""foo"": -5 }")
                .Value().ShouldBe(-5d, 0);
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
                .ShouldBeA<JsonMap>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void ObjectThenNumberProperty()
        {
            ParseJson(@"{ ""foo"": { }, ""bar"": 4 }")
                .ShouldBeA<JsonMap>()
                .And(map => map["foo"].ShouldBeA<JsonMap>())
                .And(map => map["bar"].Value().ShouldBe(4d, 0));
        }

        [Test]
        public void NumberPropertyObjectProperty()
        {
            ParseFooProperty(@"{ ""foo"": { ""bar"": 3 } }")
                .ShouldBeA<JsonMap>()
                .And.Value("bar").ShouldBe(3d, 0);
        }

        [Test]
        public void EmptyArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ ] }")
                .ShouldBeA<JsonArray>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void SingleNumberArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [1] }")
                .ShouldBeA<JsonArray>()
                .And.Single().Value().ShouldBe(1d, 0);
        }

        [Test]
        public void MultipleNumberArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ 1, 2, 3 ] }")
                .ShouldBeA<JsonArray>()
                .And.Values().ShouldMatch(new double[] { 1, 2, 3 });
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ 1, ""two"", null ] }")
                .ShouldBeA<JsonArray>()
                .And.Values().ShouldMatch(new object[] { 1d, "two", null });
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
            result.ShouldBeA<JsonMap>();
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
