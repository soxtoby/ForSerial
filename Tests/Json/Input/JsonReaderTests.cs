using System.Linq;
using json.Json;
using json.JsonObjects;
using NSubstitute;
using NUnit.Framework;

namespace json.Tests.Json
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
        public void StringProperty()
        {
            ParseFooProperty(@"{ ""foo"": ""bar"" }")
                .Value().ShouldBe("bar");
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
                .And.Values().ShouldBe<int>(new[] { 1, 2, 3 });
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            ParseFooProperty(@"{ ""foo"": [ 1, ""two"", null ] }")
                .ShouldBe<JsonArray>()
                .And.Values().ShouldBe(new object[] { 1, "two", null });
        }

        //[Test] // TODO reimplement subobject parsing
        public void ParseSubObject()
        {
            //ParseSubObjectWriter writer = new ParseSubObjectWriter();
            //Convert.From.Json(@"{""foo"":{""_type"":""bar"",""baz"":""qux""}}").WithBuilder(writer);

            //Assert.AreEqual(@"{""baz"":""qux""}", writer.SubObjectJson);
        }

        //private class ParseSubObjectWriter : TestWriter
        //{
        //    public override OutputStructure BeginStructure()
        //    {
        //        return new ParseSubObjectObject(this);
        //    }

        //    public string SubObjectJson { get; set; }
        //}

        //private class ParseSubObjectObject : NullOutputStructure
        //{
        //    private readonly ParseSubObjectWriter parentFactory;

        //    public ParseSubObjectObject(ParseSubObjectWriter parentFactory)
        //    {
        //        this.parentFactory = parentFactory;
        //    }

        //    public override bool SetType(string typeIdentifier, Reader reader)
        //    {
        //        //parentFactory.SubObjectJson = JsonStringBuilder.GetResult(reader.ReadSubStructure(JsonStringBuilder.GetDefault()));
        //        return true;
        //    }
        //}

        [Test]
        public void MaintainReferences()
        {
            Writer writer = Substitute.For<Writer>();

            JsonParser.Parse(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", writer);

            writer.Received().WriteReference(1);
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
            JsonParser.Parse(json, writer);
            return writer.Result;
        }

        // TODO probably won't need the old property context stuff, so these test can go
        //[Test]
        //public void CreatePropertyObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Json(@"{""foo"":{}}").WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        //}

        //[Test]
        //public void CreatePropertyArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Json(@"{""foo"":[]}").WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        //}

        //[Test]
        //public void CreateArrayObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Json(@"{""foo"":[{}]}").WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        //}

        //[Test]
        //public void CreateArrayArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Json(@"{""foo"":[[]]}").WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        //}
    }
}
