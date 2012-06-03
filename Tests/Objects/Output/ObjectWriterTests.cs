using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyAssertions;
using ForSerial.Json;
using ForSerial.JsonObjects;
using ForSerial.Objects;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class ObjectWriterTests
    {
        [Test]
        public void Null()
        {
            Assert.IsNull(Clone<object>(null));
        }

        [Test]
        public void Boolean()
        {
            Assert.IsTrue(Clone(true));
            Assert.IsFalse(Clone(false));
        }

        [Test]
        public void Number()
        {
            Assert.AreEqual(5, Clone(5));
        }

        [Test]
        public void String()
        {
            Assert.AreEqual("foo", Clone("foo"));
        }

        [Test]
        public void Enum()
        {
            CopyTo<TestEnum>(1d)   // Numbers coming from JsonReader will be doubles
                .ShouldBe(TestEnum.One);
        }

        [Test]
        public void List()
        {
            Clone(new List<int> { 1, 2, 3 })
                .ShouldMatch(new[] { 1, 2, 3 });
        }

        [Test]
        public void NumberAsObject()
        {
            CopyTo<object>(1)
                .ShouldBe((object)1);
        }

        [Test]
        public void BooleanProperty()
        {
            Assert.IsTrue(Clone(new BooleanPropertyClass { Boolean = true }).Boolean);
            Assert.IsFalse(Clone(new BooleanPropertyClass { Boolean = false }).Boolean);
        }

        private class BooleanPropertyClass
        {
            public bool Boolean { get; set; }
        }

        [Test]
        public void DoubleProperty()
        {
            Assert.AreEqual(1.2, Clone(new DoublePropertyClass { Double = 1.2 }).Double);
        }

        private class DoublePropertyClass
        {
            public double Double { get; set; }
        }

        [Test]
        public void IntProperty()
        {
            Assert.AreEqual(5, Clone(new IntPropertyClass { Integer = 5 }).Integer);
        }

        private class IntPropertyClass
        {
            public int Integer { get; set; }
        }

        [Test]
        public void EnumProperty()
        {
            Clone(new EnumPropertyClass { Enum = TestEnum.One })
                .Enum.ShouldBe(TestEnum.One);
        }

        private class EnumPropertyClass
        {
            public TestEnum Enum { get; set; }
        }

        private enum TestEnum
        {
            Zero,
            One
        }

        [Test]
        public void ObjectProperty()
        {
            Assert.AreEqual(5, Clone(new ObjectPropertyClass { Object = new IntPropertyClass { Integer = 5 } }).Object.Integer);
        }

        [Test]
        public void NullProperty()
        {
            Assert.IsNull(Clone(new ObjectPropertyClass()).Object);
        }

        private class ObjectPropertyClass
        {
            public IntPropertyClass Object { get; set; }
        }

        [Test]
        public void SetListProperty()
        {
            SettableListPropertyClass foo = Clone(new SettableListPropertyClass { List = new List<int> { 1, 2, 3 } });
            Assert.IsFalse(foo.GetterHasBeenAccessed);
            foo.List.ShouldMatch(new[] { 1, 2, 3 });
        }

        private class SettableListPropertyClass
        {
            public bool GetterHasBeenAccessed { get; private set; }

            private List<int> list;
            public List<int> List
            {
                get
                {
                    GetterHasBeenAccessed = true;
                    return list;
                }
                set
                {
                    list = value;
                }
            }
        }

        [Test]
        public void PopulateListProperty()
        {
            GettableListPropertyClass obj = new GettableListPropertyClass();
            obj.List.AddRange(new[] { 1, 2, 3 });

            Clone(obj).List.ShouldMatch(new[] { 1, 2, 3 });
        }

        private class GettableListPropertyClass
        {
            private readonly List<int> list = new List<int>();
            public List<int> List
            {
                get { return list; }
            }
        }

        [Test]
        public void ObjectListProperty()
        {
            ObjectListPropertyClass obj = new ObjectListPropertyClass
            {
                List = new List<IntPropertyClass>
                {
                    new IntPropertyClass { Integer = 1 },
                    new IntPropertyClass { Integer = 2 },
                    new IntPropertyClass { Integer = 3 }
                }
            };

            Clone(obj)
                .List.Select(i => i.Integer).ShouldMatch(new[] { 1, 2, 3 });
        }

        private class ObjectListPropertyClass
        {
            public List<IntPropertyClass> List { get; set; }
        }

        [Test]
        public void NestedListProperty()
        {
            NestedListPropertyClass obj = new NestedListPropertyClass
            {
                NestedList = new List<List<int>> {
                    new List<int> { 1, 2, 3 },
                    new List<int> { 4, 5, 6 }
                }
            };

            NestedListPropertyClass clone = Clone(obj);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, clone.NestedList[0]);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, clone.NestedList[1]);
        }

        private class NestedListPropertyClass
        {
            public List<List<int>> NestedList { get; set; }
        }

        [Test]
        public void ListOfSubTypes()
        {
            Clone(new List<AbstractSuperClass> { new SubClass { SuperProperty = 1, SubProperty = 2 } })
                .Single().ShouldBeA<SubClass>()
                .And(sub => sub.SuperProperty.ShouldBe(1))
                .And(sub => sub.SubProperty.ShouldBe(2));
        }

        [Test]
        public void Array()
        {
            Clone(new[] { 1, 2, 3 })
                .ShouldMatch(new[] { 1, 2, 3 });
        }

        [Test]
        public void SetDictionaryProperty()
        {
            SettableDictionaryPropertyClass foo = Clone(new SettableDictionaryPropertyClass
                {
                    Dictionary = new Dictionary<string, int>
                        {
                            { "foo", 6 }
                        }
                });
            Assert.IsFalse(foo.GetterHasBeenAccessed);
            Assert.AreEqual(6, foo.Dictionary["foo"]);
        }

        private class SettableDictionaryPropertyClass
        {
            public bool GetterHasBeenAccessed { get; private set; }

            private Dictionary<string, int> dictionary;
            public Dictionary<string, int> Dictionary
            {
                get
                {
                    GetterHasBeenAccessed = true;
                    return dictionary;
                }
                set
                {
                    dictionary = value;
                }
            }
        }

        [Test]
        public void PopulateDictionaryProperty()
        {
            var obj = new GettableDictionaryPropertyClass();
            obj.Dictionary["foo"] = 5;

            Assert.AreEqual(5, Clone(obj).Dictionary["foo"]);
        }

        private class GettableDictionaryPropertyClass
        {
            private readonly Dictionary<string, int> dictionary = new Dictionary<string, int>();
            public Dictionary<string, int> Dictionary
            {
                get { return dictionary; }
            }
        }

        [Test]
        public void ListDictionaryProperty()
        {
            var foo = Clone(new ListDictionaryPropertyClass
                {
                    Dictionary = new Dictionary<string, List<int>>
                        {
                            { "foo", new List<int> { 1, 2, 3 } }
                        }
                });

            foo.Dictionary["foo"].ShouldMatch(new[] { 1, 2, 3 });
        }

        private class ListDictionaryPropertyClass
        {
            public Dictionary<string, List<int>> Dictionary { get; set; }
        }

        [Test]
        public void NestedDictionaryProperty()
        {
            var foo = Clone(new NestedDictionaryPropertyClass
                {
                    Dictionary = new Dictionary<string, Dictionary<string, int>>
                        {
                            {
                                "bar", new Dictionary<string, int>
                                    {
                                        { "baz", 7 }
                                    }
                            }
                        }
                });

            Assert.AreEqual(7, foo.Dictionary["bar"]["baz"]);
        }

        private class NestedDictionaryPropertyClass
        {
            public Dictionary<string, Dictionary<string, int>> Dictionary { get; set; }
        }

        [Test]
        public void NumberKeyDictionaryProperty()
        {
            var foo = Clone(new NumberKeyDictionaryPropertyClass
                {
                    Dictionary = new Dictionary<int, string>
                        {
                            { 1, "one" }
                        }
                });
            Assert.AreEqual("one", foo.Dictionary[1]);
        }

        private class NumberKeyDictionaryPropertyClass
        {
            public Dictionary<int, string> Dictionary { get; set; }
        }

        [Test]
        public void PreDeserializeUpgrade()
        {
            PreDeserializeUpgradeClass original = new PreDeserializeUpgradeClass { One = 1, Two = 2 };
            StringWriter stringWriter = new StringWriter();
            JsonStringWriter jsonWriter = new JsonStringWriter(stringWriter);
            ObjectReader.Read(original, jsonWriter);
            string json = stringWriter.ToString();

            PreDeserializeUpgradeClass swapped = DeserializeJson<PreDeserializeUpgradeClass>(json);

            swapped.One.ShouldBe(2);
            swapped.Two.ShouldBe(1);
        }

        private class PreDeserializeUpgradeClass
        {
            public int One { get; set; }
            public int Two { get; set; }

            [PreDeserialize]
            public static JsonMap PreDeserialize(JsonMap json)
            {
                int one = (int)(double)json["One"].Value();
                int two = (int)(double)json["Two"].Value();
                json["One"] = new JsonValue(two);
                json["Two"] = new JsonValue(one);
                return json;
            }
        }

        [Test]
        public void MaintainObjectReferences()
        {
            IntPropertyClass original = new IntPropertyClass();
            SameReferenceTwice clone = Clone(new SameReferenceTwice(original));

            clone.One.ShouldBeThis(clone.Two);
            clone.One.ShouldNotBeThis(original);
        }

        [Test]
        public void MaintainPropertyPropertyCircularReference()
        {
            ReferencePropertyClass parent = new ReferencePropertyClass { Reference = new ReferencePropertyClass() };
            parent.Reference = parent;

            ReferencePropertyClass clone = Clone(parent);

            clone.Reference.ShouldBeA<ReferencePropertyClass>()
                .And.Reference.ShouldBeThis(clone);
        }

        [Test]
        public void MaintainConstructorPropertyCircularReference()
        {
            ReferencePropertyClass child = new ReferencePropertyClass();
            ReferenceConstructorClass parent = new ReferenceConstructorClass(child);
            child.Reference = parent;

            ReferenceConstructorClass clone = Clone(parent);

            clone.Reference.ShouldBeA<ReferencePropertyClass>()
                .And.Reference.ShouldBeThis(clone);
        }

        [Test]
        public void MaintainPropertyConstructorCircularReference()
        {
            ReferencePropertyClass parent = new ReferencePropertyClass();
            ReferenceConstructorClass child = new ReferenceConstructorClass(parent);
            parent.Reference = child;

            ReferencePropertyClass clone = Clone(parent);

            clone.Reference.ShouldBeA<ReferenceConstructorClass>()
                .And.Reference.ShouldBeThis(clone);
        }

        private class ReferenceConstructorClass
        {
            public ReferenceConstructorClass(ReferencePropertyClass reference)
            {
                Reference = reference;
            }

            public object Reference { get; private set; }
        }

        private class ReferencePropertyClass
        {
            public object Reference { get; set; }
        }

        [Test]
        public void BuildTypedObject()
        {
            DeserializeJson<IntPropertyClass>(@"{""Integer"":3}")
                .Integer.ShouldBe(3);
        }

        [Test]
        public void BuildTypedProperty()
        {
            ObjectPropertyClass obj = DeserializeJson<ObjectPropertyClass>(@"{""Object"":{""Integer"":4}}");

            Assert.AreEqual(4, obj.Object.Integer);
        }

        [Test]
        public void BuildTypedArrayProperty()
        {
            ObjectListPropertyClass obj = DeserializeJson<ObjectListPropertyClass>(@"{""List"":[{""Integer"":1},{""Integer"":2},{""Integer"":3}]}");

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.List.Select(i => i.Integer));
        }

        [Test]
        public void BuildTypedNestedArrayProperty()
        {
            NestedListPropertyClass obj = DeserializeJson<NestedListPropertyClass>(@"{""NestedList"":[[1,2,3],[4,5,6]]}");

            Assert.NotNull(obj.NestedList);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.NestedList[0]);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, obj.NestedList[1]);
        }

        [Test]
        public void InterfaceProperty()
        {
            InterfacePropertyClass obj = new InterfacePropertyClass { Property = new InterfaceImplementation { Value = 5 } };
            CopyTo<InterfacePropertyClass>(obj)
                .Property.ShouldNotBeNull()
                .And.Value.ShouldBe(5);
        }

        private class InterfacePropertyClass
        {
            public Interface Property { get; set; }
        }

        private interface Interface
        {
            int Value { get; }
        }

        private class InterfaceImplementation : Interface
        {
            public int Value { get; set; }
        }

        [Test]
        public void ConvertToSubType()
        {
            SubClass result = CopyTo<SubClass>(new SuperClass { SuperProperty = 1 });
            result.SuperProperty.ShouldBe(1);
            result.SubProperty.ShouldBe(0);
        }

        [Test]
        public void MaintainSubType()
        {
            CopyTo<AbstractSuperClass>(new SubClass { SuperProperty = 1, SubProperty = 2 })
                .ShouldBeA<SubClass>()
                .And(sub => sub.SuperProperty.ShouldBe(1))
                .And(sub => sub.SubProperty.ShouldBe(2));
        }

        [Test]
        public void ConvertToSimilarType()
        {
            CopyTo<SimilarClass>(new SubClass { SuperProperty = 1, SubProperty = 2 })
                .SubProperty.ShouldBe(2);
        }

        [Test]
        public void AbstractTypedProperty()
        {
            AbstractTypedPropertyClass original = new AbstractTypedPropertyClass { Property = new SuperClass { SuperProperty = 5 } };
            AbstractTypedPropertyClass clone = Clone(original);
            Assert.AreEqual(5, clone.Property.SuperProperty);
        }

        private class AbstractTypedPropertyClass
        {
            public AbstractSuperClass Property { get; set; }
        }

        private abstract class AbstractSuperClass
        {
            public abstract int SuperProperty { get; set; }
        }

        private class SuperClass : AbstractSuperClass
        {
            public override int SuperProperty { get; set; }
        }

        private class SubClass : SuperClass
        {
            public int SubProperty { get; set; }
        }

        private class SimilarClass
        {
            public int SubProperty { get; set; }
        }

        [Test]
        public void AbstractTypedPropertyMarkedWithSerializable()
        {
            var original = new SerializableAbstractTypedPropertyClass { Property = new SerializableAbstractImplementation { AbstractProperty = 5 } };
            var clone = Clone(original);

            Assert.AreEqual(5, clone.Property.AbstractProperty);
        }

        private class SerializableAbstractTypedPropertyClass
        {
            public SerializableAbstractClass Property { get; set; }
        }

        [Serializable]
        private abstract class SerializableAbstractClass
        {
            public abstract int AbstractProperty { get; set; }
        }

        private class SerializableAbstractImplementation : SerializableAbstractClass
        {
            public override int AbstractProperty { get; set; }
        }

        [Test]
        public void ValueTypesClonedWithoutAccessingProperties()
        {
            ValueTypePropertyClass obj = Clone(new ValueTypePropertyClass { Property = new ValueType { DontAccessMeBro = 5 } });
            Assert.AreEqual(5, obj.Property.Value);
        }

        private class ValueTypePropertyClass
        {
            public ValueType Property { get; set; }
        }

        private struct ValueType
        {
            public int Value { get; set; }

            public int DontAccessMeBro
            {
                get { throw new AssertionException("Property should not have been accessed."); }
                set { Value = value; }
            }
        }

        [Test]
        public void CloneGuid()
        {
            Guid guid = new Guid("{6a4a91ad-acd8-46ba-b539-49651c1dd31a}");
            Assert.AreEqual(guid, Clone(guid));
        }

        [Test]
        public void CloneGuidProperty()
        {
            Guid guid = new Guid("{6a4a91ad-acd8-46ba-b539-49651c1dd31a}");
            Assert.AreEqual(guid, Clone(new GuidPropertyClass { Property = guid }).Property);
        }

        [Test]
        public void BuildGuid()
        {
            DeserializeJson<Guid>(@"""4bb47128-42c1-4a75-9b0c-cd424f84d3e3""")
                .ShouldBe(new Guid("4bb47128-42c1-4a75-9b0c-cd424f84d3e3"));
        }

        private class GuidPropertyClass
        {
            public Guid Property { get; set; }
        }

        [Test]
        public void BuildDateTimeFromString()
        {
            DeserializeJson<DateTime>(@"""2001-02-02T18:05:06.007Z""")
                .ShouldBe(new DateTime(2001, 2, 3, 4, 5, 6, 7).ToUniversalTime());
        }

        [Test]
        public void BuildKeyValuePair()
        {
            KeyValuePair<string, int> result = DeserializeJson<KeyValuePair<string, int>>(@"{""Key"":""foo"", ""Value"":5}");
            result.Key.ShouldBe("foo");
            result.Value.ShouldBe(5);
        }

        [Test]
        public void AnonymousObject()
        {
            Clone(new { Property = 1 })
                .Property.ShouldBe(1);
        }

        [Test]
        public void BuildIEnumerable()
        {
            DeserializeJson<EnumerablePropertyClass>(@"{""Property"":[0,1,2]}")
                .Property.ShouldMatch(new[] { 0, 1, 2 });
        }

        private class EnumerablePropertyClass
        {
            public IEnumerable<int> Property { get; set; }
        }

        [Test]
        public void MissingPropertyIsIgnored()
        {
            MorePropertiesClass moreProperties = new MorePropertiesClass
                {
                    Integer = 1,
                    More = new IntPropertyClass { Integer = 2 }
                };

            CopyTo<IntPropertyClass>(moreProperties)
                .Integer.ShouldBe(1);
        }

        private class MorePropertiesClass
        {
            public int Integer { get; set; }
            public IntPropertyClass More { get; set; }
        }

        [Test]
        public void ConvertDoubleToNullableInt()
        {
            ObjectWriter<int?> sut = new ObjectWriter<int?>();

            sut.Write(1d);

            sut.Result.ShouldBe((int?)1);
        }

        [Test]
        public void ConvertStringToNullableGuid()
        {
            ObjectWriter<Guid?> sut = new ObjectWriter<Guid?>();

            sut.Write("d0a227da-c42a-4a5d-ad3d-adf7d0b352a2");

            sut.Result.ShouldBe((Guid?)new Guid("d0a227da-c42a-4a5d-ad3d-adf7d0b352a2"));
        }

        [Test]
        public void ExceptionWrappedInPropertyStack()
        {
            Should.Throw<WriteException>(() => DeserializeJson<PropertySetterThrowsExceptionContainer>(@"{ ""Property"": { ""ThrowsException"": 1, ""RegularProperty"": 2 } }"))
                .And.Message.ShouldContain("foo")
                        .And.ShouldContain(typeof(PropertySetterThrowsExceptionContainer).FullName + ".Property")
                        .And.ShouldContain(typeof(PropertySetterThrowsExceptionClass).FullName + ".ThrowsException");
        }

        private class PropertySetterThrowsExceptionContainer
        {
            public PropertySetterThrowsExceptionClass Property { get; set; }
        }

        private class PropertySetterThrowsExceptionClass
        {
            public int ThrowsException
            {
                get { return 0; }
                set { throw new Exception("foo"); }
            }

            public int RegularProperty { get; set; }
        }

        [Test]
        public void IEnumerableWithAddObjectMethod_CanBeDeserialized()
        {
            CopyTo<CollectionLikeClass>(new[] { 1, 2, 3 })
                .Cast<object>().ShouldMatch(new[] { 1, 2, 3 });
        }

        private class CollectionLikeClass : IEnumerable
        {
            private readonly List<int> innerList = new List<int>();

            public IEnumerator GetEnumerator()
            {
                return innerList.GetEnumerator();
            }

            public void Add(object val)
            {
                innerList.Add((int)val);
            }
        }

        [Test]
        public void IEnumerableTWithAddTMethod_CanBeDeserialized()
        {
            CopyTo<GenericCollectionLikeCLass>(new[] { 1, 2, 3 })
                .ShouldMatch(new[] { 1, 2, 3 });
        }

        private class GenericCollectionLikeCLass : IEnumerable<int>
        {
            private readonly List<int> innerList = new List<int>();

            public IEnumerator<int> GetEnumerator()
            {
                return innerList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(int val)
            {
                innerList.Add(val);
            }
        }


        private static T DeserializeJson<T>(string json)
        {
            ObjectWriter<T> writer = new ObjectWriter<T>();
            JsonReader.Read(json, writer);
            return writer.Result;
        }

        private static T Clone<T>(T obj)
        {
            return CopyTo<T>(obj);
        }

        private static T CopyTo<T>(object obj)
        {
            ObjectWriter<T> writer = new ObjectWriter<T>();
            ObjectReader.Read(obj, writer, new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.Minimal });
            return writer.Result;
        }
    }
}
