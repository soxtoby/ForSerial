using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyAssertions;
using ForSerial.Json;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class ObjectReaderTests
    {
        [Test]
        public void IntegerProperty()
        {
            Assert.AreEqual("{\"foo\":5}", ConvertToJson(new { foo = 5 }));
        }

        [Test]
        public void DecimalProperty()
        {
            Assert.AreEqual("{\"foo\":5.1}", ConvertToJson(new { foo = 5.1 }));
        }

        [Test]
        public void StringProperty()
        {
            Assert.AreEqual("{\"foo\":\"bar\"}", ConvertToJson(new { foo = "bar" }));
        }

        [Test]
        public void BooleanProperty()
        {
            Assert.AreEqual("{\"foo\":true}", ConvertToJson(new { foo = true }));
        }

        [Test]
        public void NullProperty()
        {
            Assert.AreEqual("{\"Property\":null}", ConvertToJson(new NullPropertyClass()));
        }

        private class NullPropertyClass
        {
            public object Property { get; set; }
        }

        [Test]
        public void EnumProperty()
        {
            ConvertToJson(new { foo = TestEnum.One })
                .ShouldBe(@"{""foo"":1}");
        }

        private enum TestEnum
        {
            Zero,
            One
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
            ConvertToJson(new { foo = new object[] { new { bar = 5 }, new { } } })
                .ShouldBe(@"{""foo"":[{""bar"":5},{}]}");
        }

        [Test]
        public void NestedArray()
        {
            Assert.AreEqual("{\"foo\":[[1,2],[]]}", ConvertToJson(new { foo = new[] { new[] { 1, 2 }, new int[] { } } }));
        }

        [Test]
        public void MixedArray()
        {
            ConvertToJson(new { foo = new object[] { 1, "two", new { }, new object[] { } } })
                .ShouldBe(@"{""foo"":[1,""two"",{},[]]}");
        }

        [Test]
        public void IEnumerable()
        {
            ConvertToJson(new { Property = Enumerable.Range(0, 3) })
                .ShouldBe(@"{""Property"":[0,1,2]}");
        }

        [Test]
        public void StringObjectDictionary_OutputAsRegularObject()
        {
            ConvertToJson(new Dictionary<string, object> { { "foo", "bar" } })
                .ShouldBe(@"{""foo"":""bar""}");
        }

        [Test]
        public void MaintainObjectReferences()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice(new object()), writer);

            writer.Received().WriteReference(1);
        }

        [Test]
        public void DoNotMaintainReferences_ReferencedObjectCopied()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice(new object()), writer, new ObjectParsingOptions { MaintainReferences = false });

            writer.DidNotReceive().WriteReference(Arg.Any<int>());
        }

        [Test]
        public void MaintainJsonDictionaryReferences()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice(new Dictionary<string, string>()), writer);

            writer.Received().WriteReference(1);
        }

        [Test]
        public void GuidIsNotReferenced()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice(Guid.NewGuid()), writer);

            writer.DidNotReceive().WriteReference(Arg.Any<int>());
        }

        [Test]
        public void StringsNotReferenced()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice("foo"), writer);

            writer.DidNotReceive().WriteReference(Arg.Any<int>());
        }

        [Test]
        public void StringStructsNotReferenced()
        {
            Writer writer = Substitute.For<Writer>();

            ObjectReader.Read(new SameReferenceTwice(new StringStruct { Value = "foo" }), writer);

            writer.DidNotReceive().WriteReference(Arg.Any<int>());
        }

        private struct StringStruct
        {
            public string Value { get; set; }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IgnorePropertyWithAttributeOfNonAttributeType()
        {
            StructureDefinition.IgnorePropertiesMarkedWithAttribute(typeof(IgnoredPropertyClass));
        }

        [Test]
        public void IgnorePropertyWithAttribute()
        {
            StructureDefinition.IgnorePropertiesMarkedWithAttribute(typeof(IgnoreMeAttribute));
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
        public void DateTimeParsedToString()
        {
            ConvertToJson(new DateTime(2001, 2, 3, 4, 5, 6, 7))
                .ShouldBe(@"""2001-02-02T18:05:06.007Z""");
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

        [Test]
        public void OverrideTypeHandler()
        {
            using (CurrentTypeResolver.Override(new CustomTypeResolver()))
            {
                ConvertToJson(new { foo = 5 }, new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.All })
                    .ShouldBe(@"{""_type"":""foobar"",""foo"":5}");
            }
        }

        private class CustomTypeResolver : TypeResolver
        {
            public string GetTypeIdentifier(Type type)
            {
                return "foobar";
            }

            public Type GetType(string identifier)
            {
                return null;
            }
        }

        [Test]
        public void InterfacePropertyTypeSerialized()
        {
            ConvertToSimpleTypeJson(new InterfacePropertyClass { Property = new ConcreteClass { Value = 1 } })
                .ShouldBe(@"{""_type"":""InterfacePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":1}}");
        }

        [Test]
        public void AbstractTypePropertyTypeSerialized()
        {
            ConvertToSimpleTypeJson(new AbstractTypePropertyClass { Property = new ConcreteClass { Value = 2 } })
                .ShouldBe(@"{""_type"":""AbstractTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":2}}");
        }

        [Test]
        public void KnownTypeNotSerialized()
        {
            ConvertToSimpleTypeJson(new ConcreteTypePropertyClass { Property = new ConcreteClass { Value = 3 } })
                .ShouldBe(@"{""_type"":""ConcreteTypePropertyClass"",""Property"":{""Value"":3}}");
        }

        [Test]
        public void MarkedKnownTypePropertyTypeSerialized()
        {
            ConvertToSimpleTypeJson(new MarkedConcreteTypePropertyClass { Property = new ConcreteClass { Value = 4 } })
                .ShouldBe(@"{""_type"":""MarkedConcreteTypePropertyClass"",""Property"":{""_type"":""ConcreteClass"",""Value"":4}}");
        }

        [Test]
        public void KnownTypeEnumerablePropertyTypeNotSerialized()
        {
            ConvertToSimpleTypeJson(new ConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 5 } } })
                .ShouldBe(@"{""_type"":""ConcreteTypeEnumerablePropertyClass"",""Property"":[{""Value"":5}]}");
        }

        [Test]
        public void AbstractTypeEnumerablePropertyTypeIsSerialized()
        {
            ConvertToSimpleTypeJson(new AbstractTypeEnumerablePropertyClass { Property = new List<AbstractClass> { new ConcreteClass { Value = 5 } } })
                .ShouldBe(@"{""_type"":""AbstractTypeEnumerablePropertyClass"",""Property"":[{""_type"":""ConcreteClass"",""Value"":5}]}");
        }

        [Test]
        public void MarkedKnownTypeEnumerablePropertyTypeSerialized()
        {
            ConvertToSimpleTypeJson(new MarkedConcreteTypeEnumerablePropertyClass { Property = new List<ConcreteClass> { new ConcreteClass { Value = 6 } } })
                .ShouldBe(@"{""_type"":""MarkedConcreteTypeEnumerablePropertyClass"",""Property"":[{""_type"":""ConcreteClass"",""Value"":6}]}");
        }

        [Test]
        public void KnownTypeDictionaryTypesNotSerialized()
        {
            ConvertToSimpleTypeJson(new ConcreteTypeDictionaryPropertyClass
                {
                    Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 7 }, new ConcreteClass { Value = 8 } }
                        }
                })
                .ShouldBe(@"{""_type"":""ConcreteTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":7},""Value"":{""Value"":8}}]}");
        }

        //[Test]
        public void MarkedKnownTypeKeyDictionaryTypesSerialized()
        {
            ConvertToSimpleTypeJson(new MarkedConcreteKeyTypeDictionaryPropertyClass
                {
                    Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 9 }, new ConcreteClass { Value = 10 } }
                        }
                })
                .ShouldBe(@"{""_type"":""MarkedConcreteKeyTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""_type"":""ConcreteClass"",""Value"":9},""Value"":{""Value"":10}}]}");
        }

        //[Test]
        public void MarkedKnownTypeValueDictionaryTypeSerialized()
        {
            ConvertToSimpleTypeJson(new MarkedConcreteValueTypeDictionaryPropertyClass
                {
                    Property = new Dictionary<ConcreteClass, ConcreteClass>
                        {
                            { new ConcreteClass { Value = 11 }, new ConcreteClass { Value = 12 } }
                        }
                })
                .ShouldBe(@"{""_type"":""MarkedConcreteValueTypeDictionaryPropertyClass"",""Property"":[{""Key"":{""Value"":11},""Value"":{""_type"":""ConcreteClass"",""Value"":12}}]}");
        }

        [Test]
        public void PropertyDefinitionAttribute()
        {
            ConvertToJson(new RenamedPropertyClass { Foo = 1 })
                .ShouldBe(@"{""bar"":1}");
        }

        private class RenamedPropertyClass
        {
            [RenamePropertyDefinition("bar")]
            public int Foo { get; set; }
        }

        private class RenamePropertyDefinitionAttribute : PropertyDefinitionAttribute
        {
            private readonly string name;

            public RenamePropertyDefinitionAttribute(string name)
            {
                this.name = name;
            }

            public override string Name { get { return name; } }
        }

        private static string ConvertToSimpleTypeJson(object obj)
        {
            using (CurrentTypeResolver.Override(new SimpleTypeNameTypeResolver()))
            {
                return ConvertToJson(obj, new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.Minimal });
            }
        }

        private class SimpleTypeNameTypeResolver : TypeResolver
        {
            public string GetTypeIdentifier(Type type)
            {
                return type.Name;
            }

            public Type GetType(string identifier)
            {
                return null;
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

        private class AbstractTypeEnumerablePropertyClass
        {
            public List<AbstractClass> Property { get; set; }
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

        [Test]
        public void ExceptionWrappedWithPropertyStack()
        {
            Should.Throw<ObjectReader.ObjectReadException>(() => ConvertToJson(new ObjectPropertyClass { Object = new PropertyGetterThrowsException("foo") }))
                .And.Message.ShouldContain("foo")
                        .And.ShouldContain(GetType() + "+ObjectPropertyClass.Object")
                        .And.ShouldContain(GetType() + "+PropertyGetterThrowsException.ThrowsException");
        }

        private class ObjectPropertyClass
        {
            public object Object { get; set; }
        }

        private class PropertyGetterThrowsException
        {
            private readonly string exceptionMessage;

            public PropertyGetterThrowsException(string exceptionMessage)
            {
                this.exceptionMessage = exceptionMessage;
            }

            public int ThrowsException
            {
                get { throw new Exception(exceptionMessage); }
            }
        }

        [Test]
        public void PublicGetPropertyFilter_GetOnlyPropertyNotRead()
        {
            ConvertToJson(new GetOnlyPropertyClass(), new ObjectParsingOptions { MemberAccessibility = MemberAccessibility.PublicGetSet, SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe("{}");
        }

        private class GetOnlyPropertyClass
        {
            public int Property
            {
                get { throw new AssertionException("Property should not have been read."); }
            }
        }

        [Test]
        public void PublicGetPropertyFilter_PrivateGetPropertyNotRead()
        {
            ConvertToJson(new PrivateGetPropertyClass(1), new ObjectParsingOptions { MemberAccessibility = MemberAccessibility.PublicGetSet, SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe("{}");
        }

        private class PrivateGetPropertyClass
        {
            public PrivateGetPropertyClass(int property)
            {
                Property = property;
            }

            public int Property { get; private set; }
        }

        [Test]
        public void PublicGetPropertyFilter_PublicGetSetPropertyIsRead()
        {
            ConvertToJson(new ConcreteClass { Value = 1 }, new ObjectParsingOptions { MemberAccessibility = MemberAccessibility.PublicGetSet, SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe(@"{""Value"":1}");
        }

        [Test]
        public void PublicGetPropertyFilter_PublicFieldNotRead()
        {
            ConvertToJson(new IntegerFieldClass { Field = 1 }, new ObjectParsingOptions { MemberType = MemberType.Property, SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe("{}");
        }

        [Test]
        public void PublicGetFieldFilter_PublicFieldIsRead()
        {
            ConvertToJson(new IntegerFieldClass { Field = 1 }, new ObjectParsingOptions { MemberType = MemberType.Field, SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe(@"{""Field"":1}");
        }

        private class IntegerFieldClass
        {
            public int Field;
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

        private static string ConvertToJson(object obj)
        {
            return ConvertToJson(obj, new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.None });
        }

        private static string ConvertToJson(object obj, ObjectParsingOptions options)
        {
            StringWriter stringWriter = new StringWriter();
            JsonStringWriter jsonWriter = new JsonStringWriter(stringWriter);
            ObjectReader.Read(obj, jsonWriter, options);
            return stringWriter.ToString();
        }
    }
}
