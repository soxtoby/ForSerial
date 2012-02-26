using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using json.Json;
using json.Objects;
using NUnit.Framework;

namespace json.Tests.Objects
{
    [TestFixture]
    public class ObjectReaderTests
    {
        [Test]
        public void Integer()
        {
            Assert.AreEqual("{\"foo\":5}", ConvertToJson(new { foo = 5 }));
        }

        [Test]
        public void Decimal()
        {
            Assert.AreEqual("{\"foo\":5.1}", ConvertToJson(new { foo = 5.1 }));
        }

        [Test]
        public void String()
        {
            Assert.AreEqual("{\"foo\":\"bar\"}", ConvertToJson(new { foo = "bar" }));
        }

        [Test]
        public void Boolean()
        {
            Assert.AreEqual("{\"foo\":true}", ConvertToJson(new { foo = true }));
        }

        [Test]
        public void Null()
        {
            Assert.AreEqual("{\"Property\":null}", ConvertToJson(new NullPropertyClass(), false));
        }

        private class NullPropertyClass
        {
            public object Property { get; set; }
        }

        [Test]
        public void MultipleProperties()
        {
            Assert.AreEqual("{\"foo\":1,\"bar\":2}", ConvertToJson(new { foo = 1, bar = 2 }));
        }

        [Test]
        public void NestedEmptyObject()
        {
            Assert.AreEqual("{\"foo\":{}}", ConvertToJson(new { foo = new { } }));
        }

        [Test]
        public void NestedObjectWithProperties()
        {
            Assert.AreEqual("{\"foo\":{\"bar\":5,\"baz\":\"qux\"}}", ConvertToJson(new { foo = new { bar = 5, baz = "qux" } }));
        }

        [Test]
        public void EmptyArray()
        {
            Assert.AreEqual("{\"foo\":[]}", ConvertToJson(new { foo = new object[] { } }));
        }

        [Test]
        public void SingleNumberArray()
        {
            Assert.AreEqual("{\"foo\":[1]}", ConvertToJson(new { foo = new[] { 1 } }));
        }

        [Test]
        public void MultipleNumbersArray()
        {
            Assert.AreEqual("{\"foo\":[1,2]}", ConvertToJson(new { foo = new[] { 1, 2 } }));
        }

        [Test]
        public void StringArray()
        {
            Assert.AreEqual("{\"foo\":[\"bar\",\"baz\"]}", ConvertToJson(new { foo = new[] { "bar", "baz" } }));
        }

        [Test]
        public void ObjectArray()
        {
            Assert.AreEqual("{\"foo\":[{\"bar\":5},{}]}", ConvertToJson(new { foo = new object[] { new { bar = 5 }, new { } } }));
        }

        [Test]
        public void NestedArray()
        {
            Assert.AreEqual("{\"foo\":[[1,2],[]]}", ConvertToJson(new { foo = new[] { new[] { 1, 2 }, new int[] { } } }));
        }

        [Test]
        public void MixedArray()
        {
            Assert.AreEqual("{\"foo\":[1,\"two\",{},[]]}", ConvertToJson(new { foo = new object[] { 1, "two", new { }, new object[] { } } }));
        }

        [Test]
        public void IEnumerable()
        {
            ConvertToJson(new { Property = Enumerable.Range(0, 3) })
                .ShouldBe(@"{""Property"":[0,1,2]}");
        }

        //[Test] // TODO reimplement prebuild
        public void ReadSubStructure()
        {
            //ReadSubStructureWriter writer = new ReadSubStructureWriter();
            //Convert.From.Object(new { foo = new { bar = "baz" } }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(writer);

            //Assert.AreEqual(@"{""bar"":""baz""}", writer.SubStructureJson);
        }

        //private class ReadSubStructureWriter : TestWriter
        //{
        //    public string SubStructureJson { get; set; }

        //    public override OutputStructure BeginStructure()
        //    {
        //        return new ReadSubStructureObject(this);
        //    }
        //}

        //private class ReadSubStructureObject : NullOutputStructure
        //{
        //    private readonly ReadSubStructureWriter parentFactory;

        //    public ReadSubStructureObject(ReadSubStructureWriter parentFactory)
        //    {
        //        this.parentFactory = parentFactory;
        //    }

        //    public override bool SetType(string typeIdentifier, Reader reader)
        //    {
        //        parentFactory.SubStructureJson = JsonStringBuilder.GetResult(reader.ReadSubStructure(new JsonStringBuilder()));
        //        return true;
        //    }
        //}

        [Test]
        public void StringObjectDictionary_OutputAsRegularObject()
        {
            ConvertToJson(new Dictionary<string, object> { { "foo", "bar" } })
                .ShouldBe(@"{""foo"":""bar""}");
        }

        //[Test] //TODO reimplement object references
        public void MaintainReferences()
        {
            //SameReferenceTwice foo = new SameReferenceTwice(new object());
            //var testBuilder = new WatchForReferenceBuilder();
            //Convert.From.Object(foo).WithBuilder(testBuilder);

            //Assert.NotNull(testBuilder.ReferencedObject);
        }

        // [Test] // TODO reimplement object references
        public void ValueTypesNotReferenced()
        {
            //SameReferenceTwice obj = new SameReferenceTwice(new KeyValuePair<int, int>(1, 2));
            //var testBuilder = new WatchForReferenceBuilder();
            //Convert.From.Object(obj).WithBuilder(testBuilder);

            //Assert.IsNull(testBuilder.ReferencedObject);
        }

        // [Test] // TODO reimplement object references
        public void StringsNotReferenced()
        {
            //SameReferenceTwice obj = new SameReferenceTwice("foo");
            //var testBuilder = new WatchForReferenceBuilder();
            //Convert.From.Object(obj).WithBuilder(testBuilder);

            //Assert.IsNull(testBuilder.ReferencedObject);
        }

        // TODO remove if not using objcet/array/property contexts anymore
        //[Test]
        //public void CreatePropertyObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Object(new { foo = new object() }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        //}

        //[Test]
        //public void CreatePropertyArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Object(new { foo = new object[] { } }, new ObjectParsingOptions { SerializeAllTypes = true }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        //}

        //[Test]
        //public void CreateArrayObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Object(new[] { new object() }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        //}

        //[Test]
        //public void CreateArrayArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.Object(new[] { new object[] { } }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        //}

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
            Assert.AreEqual(@"{""Serialized"":2}", ConvertToJson(new IgnoredPropertyClass { Ignored = 1, Serialized = 2 }));
        }

        private class IgnoreMeAttribute : Attribute { }

        private class IgnoredPropertyClass
        {
            [IgnoreMe]
            public int Ignored { get; set; }

            public int Serialized { get; set; }
        }

        [Test]
        public void ReadGuidAsString()
        {
            Guid guid = new Guid("{ceac23f4-9a28-4dc5-856a-1411511a0a88}");
            Assert.AreEqual(@"{""foo"":""ceac23f4-9a28-4dc5-856a-1411511a0a88""}", ConvertToJson(new { foo = guid }));
        }

        [Test]
        public void DateTimeParsedToNumber()
        {
            ConvertToJson(new DateTime(2001, 2, 3, 4, 5, 6, 7))
                .ShouldBe("981137106007");
        }

        // [Test] // TODO reimplement object references
        public void ValueTypeParsedToValue()
        {
            //Convert.From.Object(new ValueType()).WithBuilder(new ValueOnlyWriter());
        }

        private struct ValueType { }

        private class ValueOnlyWriter : TestWriter
        {
            public override OutputStructure BeginStructure()
            {
                throw new AssertionException("Tried to create an object.");
            }

            public override SequenceOutput BeginSequence()
            {
                throw new AssertionException("Tried to create an array.");
            }

            public override OutputStructure CreateReference(OutputStructure outputStructure)
            {
                throw new AssertionException("Tried to create a reference.");
            }
        }

        [Test]
        public void StaticPropertyIsNotSerialized()
        {
            StaticPropertyClass.StaticProperty = 6;
            Assert.AreEqual(@"{""Property"":5}", ConvertToJson(new StaticPropertyClass { Property = 5 }));
        }

        private class StaticPropertyClass
        {
            public int Property { get; set; }
            public static int StaticProperty { get; set; }
        }

        //[Test] //TODO reimplement SetType
        public void OverrideTypeHandler()
        {
            //using (CurrentTypeHandler.Override(new CustomTypeHandler()))
            //{
            //    string json = Convert.From.Object(new { foo = 5 }, new ObjectParsingOptions { SerializeAllTypes = true }).ToTypedJson();
            //    Assert.AreEqual(@"{""_type"":""foobar"",""foo"":5}", json);
            //}
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

        //[Test] //TODO reimplement SetType
        public void InterfacePropertyTypeSerialized()
        {
            string json = ConvertToSimpleTypeJson(new InterfacePropertyClass { Property = new ConcreteClass { Value = 1 } });
            Assert.AreEqual(@"{""_type"":""InterfacePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":1}}", json);
        }

        //[Test] //TODO reimplement SetType
        public void AbstractTypePropertyTypeSerialized()
        {
            string json = ConvertToSimpleTypeJson(new AbstractTypePropertyClass { Property = new ConcreteClass { Value = 2 } });
            Assert.AreEqual(@"{""_type"":""AbstractTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":2}}", json);
        }

        //[Test] //TODO reimplement SetType
        public void KnownTypeNotSerialized()
        {
            string json = ConvertToSimpleTypeJson(new ConcreteTypePropertyClass { Property = new ConcreteClass { Value = 3 } });
            Assert.AreEqual(@"{""_type"":""ConcreteTypePropertyClass"",""Property"":{""Value"":3}}", json);
        }

        //[Test] //TODO reimplement SetType
        public void MarkedKnownTypePropertyTypeSerialized()
        {
            string json = ConvertToSimpleTypeJson(new MarkedConcreteTypePropertyClass { Property = new ConcreteClass { Value = 4 } });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":4}}", json);
        }

        //[Test] //TODO reimplement SetType
        public void KnownTypeEnumerablePropertyTypeNotSerialized()
        {
            string json = ConvertToSimpleTypeJson(new ConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 5 } } });
            Assert.AreEqual(@"{""_type"":""ConcreteTypeEnumerablePropertyClass"",""Property"":[{""Value"":5}]}", json);
        }

        //[Test] //TODO reimplement SetType
        public void MarkedKnownTypeEnumerablePropertyTypeSerialized()
        {
            string json = ConvertToSimpleTypeJson(new MarkedConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 6 } } });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteTypeEnumerablePropertyClass"",""Property"":[{""_type"":""ConcreteClass"",""Value"":6}]}", json);
        }

        //[Test] //TODO reimplement SetType
        public void KnownTypeDictionaryTypesNotSerialized()
        {
            string json = ConvertToSimpleTypeJson(new ConcreteTypeDictionaryPropertyClass
            {
                Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 7 }, new ConcreteClass { Value = 8 } }
                        }
            });
            Assert.AreEqual(@"{""_type"":""ConcreteTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":7},""Value"":{""Value"":8}}]}", json);
        }

        //[Test]
        public void MarkedKnownTypeKeyDictionaryTypesSerialized()
        {
            string json = ConvertToSimpleTypeJson(new MarkedConcreteKeyTypeDictionaryPropertyClass
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
            string json = ConvertToSimpleTypeJson(new MarkedConcreteValueTypeDictionaryPropertyClass
                {
                    Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 11 }, new ConcreteClass { Value = 12 } }
                        }
                });
            Assert.AreEqual(@"{""_type"":""MarkedConcreteValueTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":11},""Value"":{""_type"":""ConcreteClass"",""Value"":12}}]}", json);
        }

        private static string ConvertToSimpleTypeJson(object obj)
        {
            using (CurrentTypeHandler.Override(new SimpleTypeNameTypeHandler()))
            {
                return ConvertToJson(obj);
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


        private static string ConvertToJson(object obj, bool serializeAllTypes = true)
        {
            var options = new ObjectParsingOptions { SerializeAllTypes = serializeAllTypes };
            StringWriter stringWriter = new StringWriter();
            JsonStringWriter jsonWriter = new JsonStringWriter(stringWriter);
            ObjectReader.Read(obj, jsonWriter, options);
            return stringWriter.ToString();
        }
    }
}
