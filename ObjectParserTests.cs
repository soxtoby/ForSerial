
using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
namespace json
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
            Assert.AreEqual("{\"foo\":{}}", ParseToJson(new { foo = new {  } }));
        }

        [Test]
        public void NestedObjectWithProperties()
        {
            Assert.AreEqual("{\"foo\":{\"bar\":5,\"baz\":\"qux\"}}", ParseToJson(new { foo = new { bar = 5, baz = "qux" } }));
        }

        [Test]
        public void EmptyArray()
        {
            Assert.AreEqual("{\"foo\":[]}", ParseToJson(new { foo = new object[] {  } }));
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
            Assert.AreEqual("{\"foo\":[{\"bar\":5},{}]}", ParseToJson(new { foo = new object[] { new { bar = 5 }, new {  } } }));
        }

        [Test]
        public void NestedArray()
        {
            Assert.AreEqual("{\"foo\":[[1,2],[]]}", ParseToJson(new { foo = new int[][] { new[] { 1, 2 }, new int[] {  } } }));
        }

        [Test]
        public void MixedArray()
        {
            Assert.AreEqual("{\"foo\":[1,\"two\",{},[]]}", ParseToJson(new { foo = new object[] { 1, "two", new {  }, new object[] {  } } }));
        }

        [Test]
        public void Number_IsWrappedInObject()
        {
            Assert.AreEqual("{\"value\":5}", ParseToJson(5));
        }

        [Test]
        public void String_IsWrappedInObject()
        {
            Assert.AreEqual("{\"value\":\"foo\"}", ParseToJson("foo"));
        }

        [Test]
        public void Array_IsWrappedInObject()
        {
            Assert.AreEqual("{\"items\":[]}", ParseToJson(new object[] {  }));
        }

        [Test]
        [ExpectedException(typeof(ObjectParser.ObjectParserException))]
        public void UnknownTypeCode()
        {
            ParseToJson(new { DBNull.Value });
        }


        private string ParseToJson(object obj)
        {
            ParseObject json = ObjectParser.Parse(obj, new JsonStringBuilder());
            return JsonStringBuilder.GetResult(json);
        }
    }
}
