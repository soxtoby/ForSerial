using System;
using System.Collections.Generic;
using json.Json;
using NUnit.Framework;

namespace json.Objects
{
    [TestFixture]
    public class ObjectParserTests
    {
        [Test]
        public void Integer()
        {
            Assert.AreEqual("{\"foo\":5}", ParseToJson(new { foo = 5 }));
        }

        [Test]
        public void Decimal()
        {
            Assert.AreEqual("{\"foo\":5.1}", ParseToJson(new { foo = 5.1 }));
        }

        [Test]
        public void String()
        {
            Assert.AreEqual("{\"foo\":\"bar\"}", ParseToJson(new { foo = "bar" }));
        }

        [Test]
        public void Boolean()
        {
            Assert.AreEqual("{\"foo\":true}", ParseToJson(new { foo = true }));
        }

        [Test]
        public void Null()
        {
            Assert.AreEqual("{\"foo\":null}", ParseToJson(new { foo = (object)null }));
        }

        [Test]
        public void MultipleProperties()
        {
            Assert.AreEqual("{\"foo\":1,\"bar\":2}", ParseToJson(new { foo = 1, bar = 2 }));
        }

        [Test]
        public void NestedEmptyObject()
        {
            Assert.AreEqual("{\"foo\":{}}", ParseToJson(new { foo = new { } }));
        }

        [Test]
        public void NestedObjectWithProperties()
        {
            Assert.AreEqual("{\"foo\":{\"bar\":5,\"baz\":\"qux\"}}", ParseToJson(new { foo = new { bar = 5, baz = "qux" } }));
        }

        [Test]
        public void EmptyArray()
        {
            Assert.AreEqual("{\"foo\":[]}", ParseToJson(new { foo = new object[] { } }));
        }

        [Test]
        public void SingleNumberArray()
        {
            Assert.AreEqual("{\"foo\":[1]}", ParseToJson(new { foo = new[] { 1 } }));
        }

        [Test]
        public void MultipleNumbersArray()
        {
            Assert.AreEqual("{\"foo\":[1,2]}", ParseToJson(new { foo = new[] { 1, 2 } }));
        }

        [Test]
        public void StringArray()
        {
            Assert.AreEqual("{\"foo\":[\"bar\",\"baz\"]}", ParseToJson(new { foo = new[] { "bar", "baz" } }));
        }

        [Test]
        public void ObjectArray()
        {
            Assert.AreEqual("{\"foo\":[{\"bar\":5},{}]}", ParseToJson(new { foo = new object[] { new { bar = 5 }, new { } } }));
        }

        [Test]
        public void NestedArray()
        {
            Assert.AreEqual("{\"foo\":[[1,2],[]]}", ParseToJson(new { foo = new[] { new[] { 1, 2 }, new int[] { } } }));
        }

        [Test]
        public void MixedArray()
        {
            Assert.AreEqual("{\"foo\":[1,\"two\",{},[]]}", ParseToJson(new { foo = new object[] { 1, "two", new { }, new object[] { } } }));
        }

        [Test]
        public void ParseSubObject()
        {
            ParseSubObjectValueFactory valueFactory = new ParseSubObjectValueFactory();
            Parse.From.Object(new { foo = new { bar = "baz" } }, ObjectParser.Options.SerializeOneWayTypes).WithBuilder(valueFactory);

            Assert.AreEqual(@"{""bar"":""baz""}", valueFactory.SubObjectJson);
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
                parentFactory.SubObjectJson = JsonStringBuilder.GetResult(parser.ParseSubObject(JsonStringBuilder.Default));
                return true;
            }
        }

        [Test]
        public void ClassWithNoDefaultConstructor_IsNotSerialized()
        {
            Assert.AreEqual("{}", Parse.From.Object(new { foo = new DefaultConstructorChallengedClass(5) }).ToJson());
        }

        private class DefaultConstructorChallengedClass
        {
            public DefaultConstructorChallengedClass(int foo)
            {
                Foo = foo;
            }

            public int Foo { get; set; }
        }

        [Test]
        public void StringObjectDictionary_OutputAsRegularObject()
        {
            var json = Parse.From.Object(new Dictionary<string, object> { { "foo", "bar" } }).ToJson();
            Assert.AreEqual(@"{""foo"":""bar""}", json);
        }

        [Test]
        [ExpectedException(typeof(ObjectParser.UnknownTypeCode))]
        public void UnknownTypeCode()
        {
            ParseToJson(new { DBNull.Value });
        }

        private static string ParseToJson(object obj)
        {
            return Parse.From.Object(obj, ObjectParser.Options.SerializeOneWayTypes).ToJson();
        }

        [Test]
        public void MaintainReferences()
        {
            SameReferenceTwice foo = new SameReferenceTwice(new object());
            var testBuilder = new WatchForReferenceBuilder();
            Parse.From.Object(foo).WithBuilder(testBuilder);

            Assert.NotNull(testBuilder.ReferencedObject);
        }
    }
}
