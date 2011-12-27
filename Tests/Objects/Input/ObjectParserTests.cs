using System;
using System.Collections.Generic;
using System.Linq;
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
        public void IEnumerable()
        {
            string json = Parse.From
                .Object(new { Property = Enumerable.Range(0, 3) })
                .ToJson();
            Assert.AreEqual(@"{""Property"":[0,1,2]}", json);
        }

        [Test]
        public void ParseSubObject()
        {
            ParseSubObjectValueFactory valueFactory = new ParseSubObjectValueFactory();
            Parse.From.Object(new { foo = new { bar = "baz" } }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

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
            Parse.From.Object(new { foo = new object() }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        }

        [Test]
        public void CreatePropertyArray()
        {
            var valueFactory = new CustomCreateValueFactory();
            Parse.From.Object(new { foo = new object[] { } }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

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
            using (CurrentTypeHandler.Override(new CustomTypeHandler()))
            {
                string json = Parse.From.Object(new { foo = 5 }, new ObjectParsingOptions { SerializeAllTypes = true }).ToTypedJson();
                Assert.AreEqual(@"{""_type"":""foobar"",""foo"":5}", json);
            }
        }

        private class CustomTypeHandler : TypeHandler
        {
            public string GetTypeIdentifier(Type type)
            {
                return "foobar";
            }

            public TypeDefinition GetTypeDefinition(string typeIdentifier)
            {
                return DefaultTypeHandler.Instance.GetTypeDefinition(typeIdentifier);
            }

            public TypeDefinition GetTypeDefinition(Type type)
            {
                return DefaultTypeHandler.Instance.GetTypeDefinition(type);
            }
        }

        [Test]
        public void InterfacePropertyTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new InterfacePropertyClass { Property = new ConcreteClass { Value = 1 } });
            Assert.AreEqual(@"{""_type"":""InterfacePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":1}}", json);
        }

        [Test]
        public void AbstractTypePropertyTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new AbstractTypePropertyClass { Property = new ConcreteClass { Value = 2 } });
            Assert.AreEqual(@"{""_type"":""AbstractTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":2}}", json);
        }

        [Test]
        public void KnownTypeNotSerialized()
        {
            string json = ParseToSimpleTypeJson(new ConcreteTypePropertyClass { Property = new ConcreteClass { Value = 3 } });
            Assert.AreEqual(@"{""_type"":""ConcreteTypePropertyClass"",""Property"":{""Value"":3}}", json);
        }

        [Test]
        public void MarkedKnownTypePropertyTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new MarkedConcreteTypePropertyClass { Property = new ConcreteClass { Value = 4 } });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":4}}", json);
        }

        [Test]
        public void KnownTypeEnumerablePropertyTypeNotSerialized()
        {
            string json = ParseToSimpleTypeJson(new ConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 5 } } });
            Assert.AreEqual(@"{""_type"":""ConcreteTypeEnumerablePropertyClass"",""Property"":[{""Value"":5}]}", json);
        }

        [Test]
        public void MarkedKnownTypeEnumerablePropertyTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new MarkedConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 6 } } });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteTypeEnumerablePropertyClass"",""Property"":[{""_type"":""ConcreteClass"",""Value"":6}]}", json);
        }

        [Test]
        public void KnownTypeDictionaryTypeNotSerialized()
        {
            string json = ParseToSimpleTypeJson(new ConcreteTypeDictionaryPropertyClass
            {
                Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 7 }, new ConcreteClass { Value = 8 } }
                        }
            });
            Assert.AreEqual(@"{""_type"":""ConcreteTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":7},""Value"":{""Value"":8}}]}", json);
        }

        //[Test]
        public void MarkedKnownTypeKeyDictionaryTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new MarkedConcreteKeyTypeDictionaryPropertyClass
            {
                Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 9 }, new ConcreteClass { Value = 10 } }
                        }
            });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteKeyTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""_type"":""ConcreteClass"",""Value"":9},""Value"":{""Value"":10}}]}", json);
        }

        //[Test]
        public void MarkedKnownTypeValueDictionaryTypeSerialized()
        {
            string json = ParseToSimpleTypeJson(new MarkedConcreteValueTypeDictionaryPropertyClass
                {
                    Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 11 }, new ConcreteClass { Value = 12 } }
                        }
                });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteValueTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":11},""Value"":{""_type"":""ConcreteClass"",""Value"":12}}]}", json);
        }

        private static string ParseToSimpleTypeJson(object obj)
        {
            using (CurrentTypeHandler.Override(new SimpleTypeNameTypeHandler()))
            {
                return Parse.From.Object(obj).ToTypedJson();
            }
        }

        private class SimpleTypeNameTypeHandler : TypeHandler
        {
            public string GetTypeIdentifier(Type type)
            {
                return type.Name;
            }

            public TypeDefinition GetTypeDefinition(string typeIdentifier)
            {
                return DefaultTypeHandler.Instance.GetTypeDefinition(typeIdentifier);
            }

            public TypeDefinition GetTypeDefinition(Type type)
            {
                return DefaultTypeHandler.Instance.GetTypeDefinition(type);
            }
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

        private class MarkedConcreteTypePropertyClass
        {
            [SerializeType]
            public ConcreteClass Property { get; set; }
        }

        private class ConcreteTypeEnumerablePropertyClass
        {
            public List<ConcreteClass> Property { get; set; }
        }

        private class MarkedConcreteTypeEnumerablePropertyClass
        {
            [SerializeType]
            public List<ConcreteClass> Property { get; set; }
        }

        private class ConcreteTypeDictionaryPropertyClass
        {
            public Dictionary<ConcreteClass, ConcreteClass> Property { get; set; }
        }

        private class MarkedConcreteKeyTypeDictionaryPropertyClass
        {
            [SerializeKeyType]
            public Dictionary<ConcreteClass, ConcreteClass> Property { get; set; }
        }

        private class MarkedConcreteValueTypeDictionaryPropertyClass
        {
            [SerializeValueType]
            public Dictionary<ConcreteClass, ConcreteClass> Property { get; set; }
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
            var options = new ObjectParsingOptions { SerializeAllTypes = serializeAllTypes };
            return Parse.From.Object(obj, options).ToJson();
        }
    }
}
