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
            Assert.AreEqual("{\"Property\":null}", ParseToJson(new NullPropertyClass(), false));
        }

        private class NullPropertyClass
        {
            public object Property { get; set; }
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
            Parse.From.Object(new { foo = new { bar = "baz" } }, new CustomObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

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

        private class ParseSubObjectObject : NullParseObject
        {
            private readonly ParseSubObjectValueFactory parentFactory;

            public ParseSubObjectObject(ParseSubObjectValueFactory parentFactory)
            {
                this.parentFactory = parentFactory;
            }

            public override bool SetType(string typeIdentifier, Parser parser)
            {
                parentFactory.SubObjectJson = JsonStringBuilder.GetResult(parser.ParseSubObject(new JsonStringBuilder()));
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
        public void MaintainReferences()
        {
            SameReferenceTwice foo = new SameReferenceTwice(new object());
            var testBuilder = new WatchForReferenceBuilder();
            Parse.From.Object(foo).WithBuilder(testBuilder);

            Assert.NotNull(testBuilder.ReferencedObject);
        }

        [Test]
        public void CreatePropertyObject()
        {
            var valueFactory = new CustomCreateValueFactory();
            Parse.From.Object(new { foo = new object() }, new CustomObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        }

        [Test]
        public void CreatePropertyArray()
        {
            var valueFactory = new CustomCreateValueFactory();
            Parse.From.Object(new { foo = new object[] { } }, new CustomObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        }

        [Test]
        public void CreateArrayObject()
        {
            var valueFactory = new CustomCreateValueFactory();
            Parse.From.Object(new[] { new object() }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        }

        [Test]
        public void CreateArrayArray()
        {
            var valueFactory = new CustomCreateValueFactory();
            Parse.From.Object(new[] { new object[] { } }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IgnorePropertyWithAttributeOfNonAttributeType()
        {
            TypeDefinition.IgnorePropertiesMarkedWithAttribute(typeof(IgnoredPropertyClass));
        }

        [Test]
        public void IgnorePropertyWithAttribute()
        {
            TypeDefinition.IgnorePropertiesMarkedWithAttribute(typeof(IgnoreMeAttribute));
            Assert.AreEqual(@"{""Serialized"":2}", ParseToJson(new IgnoredPropertyClass { Ignored = 1, Serialized = 2 }));
        }

        private class IgnoreMeAttribute : Attribute { }

        private class IgnoredPropertyClass
        {
            [IgnoreMe]
            public int Ignored { get; set; }

            public int Serialized { get; set; }
        }

        [Test]
        public void UnserializablePropertyIsNotGotten()
        {
            Assert.AreEqual("{}", ParseToJson(new PropertyThrowsOnGet(), false));
        }

        private class PropertyThrowsOnGet
        {
            public int Property
            {
                get { throw new InvalidOperationException(); }
            }
        }

        [Test]
        public void ParseGuid()
        {
            Guid guid = new Guid("{ceac23f4-9a28-4dc5-856a-1411511a0a88}");
            Assert.AreEqual(@"{""foo"":""ceac23f4-9a28-4dc5-856a-1411511a0a88""}", ParseToJson(new { foo = guid }));
        }

        [Test]
        public void ValueTypeParsedToValue()
        {
            Parse.From.Object(new ValueType()).WithBuilder(new ParseToValueFactory());
        }

        private struct ValueType { }

        private class ParseToValueFactory : TestValueFactory
        {
            public override ParseObject CreateObject()
            {
                throw new AssertionException("Tried to create an object.");
            }

            public override ParseArray CreateArray()
            {
                throw new AssertionException("Tried to create an array.");
            }

            public override ParseObject CreateReference(ParseObject parseObject)
            {
                throw new AssertionException("Tried to create a reference.");
            }
        }

        [Test]
        public void StaticPropertyIsNotSerialized()
        {
            StaticPropertyClass.StaticProperty = 6;
            Assert.AreEqual(@"{""Property"":5}", ParseToJson(new StaticPropertyClass { Property = 5 }));
        }

        private class StaticPropertyClass
        {
            public int Property { get; set; }
            public static int StaticProperty { get; set; }
        }

        [Test]
        public void OverrideTypeHandler()
        {
            string json = Parse.From.Object(new { foo = 5 }, new CustomObjectParsingOptions { SerializeAllTypes = true, TypeHandler = new CustomTypeHandler() }).ToTypedJson();
            Assert.AreEqual(@"{""_type"":""foobar"",""foo"":5}", json);
        }

        private class CustomTypeHandler : TypeHandler
        {
            public string GetTypeIdentifier(Type type)
            {
                return "foobar";
            }

            public TypeDefinition GetTypeDefinition(Type type)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void InterfacePropertyTypeSerialized()
        {
            string json = Parse.From
                .Object(new InterfacePropertyClass { Property = new ConcreteClass { Value = 1 } })
                .ToTypedJson();
        }

        [Test]
        public void UnnecessaryTypesNotSerialized()
        {

        }

        private class InterfacePropertyClass
        {
            public Interface Property { get; set; }
        }

        private class AbstractTypePropertyClass
        {
            public AbstractClass Property { get; set; }
        }

        private class ConcreteTypePropertyClass
        {
            public ConcreteClass Property { get; set; }
        }

        private interface Interface
        {
            int Value { get; set; }
        }

        private abstract class AbstractClass : Interface
        {
            public abstract int Value { get; set; }
        }

        private class ConcreteClass : AbstractClass
        {
            public override int Value { get; set; }
        }


        private static string ParseToJson(object obj, bool serializeAllTypes = true)
        {
            var options = new CustomObjectParsingOptions { SerializeAllTypes = serializeAllTypes };
            return Parse.From.Object(obj, options).ToJson();
        }
    }
}
