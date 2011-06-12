using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace json
{
    public class ObjectParser
    {
        private ParseValueFactory valueFactory;

        private ObjectParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseObject Parse(object obj, ParseValueFactory valueFactory)
        {
            ObjectParser parser = new ObjectParser(valueFactory);

            ParseValue value = parser.ParseValue(obj);

            return value.AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            switch (Type.GetTypeCode(input.GetType()))
            {
                case TypeCode.Object:
                    IEnumerable enumerable = input as IEnumerable;
                    if (enumerable != null)
                        return ParseArray(enumerable);
                    return ParseObject(input);
                
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
                    throw new ObjectParserException("Unknown TypeCode.", input);
            }
        }

        private ParseObject ParseObject(object obj)
        {
            if (!(obj is Object))
                throw new ObjectParserException("Expected Object.", obj);

            ParseObject output = valueFactory.CreateObject();

            Type type = obj.GetType();
            output.TypeIdentifier = GetTypeIdentifier(type);

            var properties = type.GetProperties()
                .Select(p => new {
                    Name = p.Name,
                    Get = p.GetGetMethod()
                })
                .Where(p => p.Get != null);

            foreach (var property in properties)
            {
                ParseValue value = ParseValue(property.Get.Invoke(obj, new object[] {  }));
                value.AddToObject(output, property.Name);
            }

            var fields = type.GetFields()
                .Where(f => f.IsPublic && !f.IsInitOnly && !f.IsNotSerialized);

            foreach (var field in fields)
            {
                ParseValue value = ParseValue(field.GetValue(obj));
                value.AddToObject(output, field.Name);
            }

            return output;
        }

        private ParseArray ParseArray(IEnumerable input)
        {
            ParseArray array = valueFactory.CreateArray();
            foreach (object item in input)
            {
                ParseValue value = ParseValue(item);
                value.AddToArray(array);
            }
            return array;
        }


        public static string GetTypeIdentifier(Type type)
        {
            return type.FullName;
        }


        public class ObjectParserException : Exception
        {
            public ObjectParserException(string message, object obj)
                : base(message + " Type: " + obj.GetType().FullName)
            { }
        }
    }

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
            return ObjectParser.Parse(obj, new JsonOutput()).ToString();
        }
    }
}

