using System;
using System.Collections.Generic;
using System.Linq;
using json.Json;
using json.Objects;
using NUnit.Framework;

namespace json.Tests.Objects
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
        public void Array()
        {
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, Clone(new List<int> { 1, 2, 3 }));
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
        public void SetArrayProperty()
        {
            SettableListPropertyClass foo = Clone(new SettableListPropertyClass { Array = new List<int> { 1, 2, 3 } });
            Assert.IsFalse(foo.GetterHasBeenAccessed);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, foo.Array);
        }

        private class SettableListPropertyClass
        {
            public bool GetterHasBeenAccessed { get; private set; }

            private List<int> list;
            public List<int> Array
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
        public void PopulateArrayProperty()
        {
            GettableListPropertyClass obj = new GettableListPropertyClass();
            obj.Array.AddRange(new[] { 1, 2, 3 });

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, Clone(obj).Array);
        }

        private class GettableListPropertyClass
        {
            private readonly List<int> list = new List<int>();
            public List<int> Array
            {
                get { return list; }
            }
        }

        [Test]
        public void ObjectArrayProperty()
        {
            ObjectArrayPropertyClass obj = new ObjectArrayPropertyClass
            {
                Array = new List<IntPropertyClass>
                {
                    new IntPropertyClass { Integer = 1 },
                    new IntPropertyClass { Integer = 2 },
                    new IntPropertyClass { Integer = 3 }
                }
            };

            Clone(obj)
                .Array.Select(i => i.Integer).ShouldBe(new[] { 1, 2, 3 });
        }

        private class ObjectArrayPropertyClass
        {
            public List<IntPropertyClass> Array { get; set; }
        }

        [Test]
        public void NestedArrayProperty()
        {
            NestedArrayPropertyClass obj = new NestedArrayPropertyClass
            {
                NestedArray = new List<List<int>> {
                    new List<int> { 1, 2, 3 },
                    new List<int> { 4, 5, 6 }
                }
            };

            NestedArrayPropertyClass clone = Clone(obj);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, clone.NestedArray[0]);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, clone.NestedArray[1]);
        }

        private class NestedArrayPropertyClass
        {
            public List<List<int>> NestedArray { get; set; }
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

            foo.Dictionary["foo"].ShouldBe(new[] { 1, 2, 3 });
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

        //[Test]    // TODO reimplement typing
        //[ExpectedException(typeof(TypedObjectBase.PropertyTypeMismatch))]
        //public void PropertyTypeMismatch()
        //{
        //var obj = new { Property = new BooleanPropertyClass() }, new ObjectParsingOptions { SerializeAllTypes = true, SerializeTypeInformation = true };

        //StringWriter stringWriter = new StringWriter();
        //JsonStringWriter jsonWriter = new JsonStringWriter(stringWriter);
        //ObjectReader.Read(obj, jsonWriter);

        //string json = stringWriter.ToString();
        //DeserializeJson<InterfacePropertyClass>(json);
        //}

        //[Test] // TODO reimplement SetType
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddNullToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":null}");
        //}

        // TODO not sure if want
        //[Test]
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddBooleanToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":true}");
        //}

        //[Test]
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddNumberToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":5}");
        //}

        //[Test]
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddStringToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":""bar""}");
        //}

        //[Test]
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddObjectToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":{}}");
        //}

        //[Test]
        //[ExpectedException(typeof(TypedObjectOutputStructure.ObjectNotInitialized))]
        //public void AddArrayToUntypedObject()
        //{
        //    DeserializeJson<object>(@"{""foo"":[]}");
        //}

        //[Test]
        //[ExpectedException(typeof(TypedObjectBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            //TypedObjectBuilder.GetResult<object>(NullOutputStructure.Instance);// TODO remove
        }

        //[Test]
        //[ExpectedException(typeof(TypedObjectBuilder.UnknownRootArrayType))]
        public void UnknownRootArrayType()
        {
            // TODO probably remove
            //Convert.From.Object(new object[] { }).WithBuilder(TypedObjectBuilder.GetGenericInstance());
        }

        //[Test] // TODO reimplement prebuild
        //public void PreDeserializeUpgrade()
        //{
        //    string json = Convert.From.Object(new PreDeserializeUpgradeClass { One = 1, Two = 2 }).ToTypedJson();
        //    PreDeserializeUpgradeClass obj = Convert.From.Json(json).ToObject<PreDeserializeUpgradeClass>();

        //    Assert.AreEqual(2, obj.One);
        //    Assert.AreEqual(1, obj.Two);
        //}

        //private class PreDeserializeUpgradeClass
        //{
        //    public int One { get; set; }
        //    public int Two { get; set; }

        //    [PreDeserialize]
        //    public JsonObject PreDeserialize(JsonObject json)
        //    {
        //        int one = (int)(json.Get("One") as double? ?? 0);
        //        int two = (int)(json.Get("Two") as double? ?? 0);
        //        json["One"] = two;
        //        json["Two"] = one;
        //        return json;
        //    }
        //}

        [Test]
        public void MaintainObjectReferences()
        {
            IntPropertyClass original = new IntPropertyClass();
            SameReferenceTwice clone = Clone(new SameReferenceTwice(original));

            clone.One.ShouldBeSameAs(clone.Two);
            clone.One.ShouldNotBeSameAs(original);
        }

        // TODO add test for circular reference

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
            ObjectArrayPropertyClass obj = DeserializeJson<ObjectArrayPropertyClass>(@"{""Array"":[{""Integer"":1},{""Integer"":2},{""Integer"":3}]}");

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.Array.Select(i => i.Integer));
        }

        [Test]
        public void BuildTypedNestedArrayProperty()
        {
            NestedArrayPropertyClass obj = DeserializeJson<NestedArrayPropertyClass>(@"{""NestedArray"":[[1,2,3],[4,5,6]]}");

            Assert.NotNull(obj.NestedArray);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.NestedArray[0]);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, obj.NestedArray[1]);
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
                .ShouldBe<SubClass>()
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
        public void BuildDateTimeFromNumber()
        {
            DeserializeJson<DateTime>("981137106007")
                .ShouldBe(new DateTime(2001, 2, 3, 4, 5, 6, 7));
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
                .Property.ShouldBe(new[] { 0, 1, 2 });
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

        private static T DeserializeJson<T>(string json)
        {
            ObjectWriter<T> writer = new ObjectWriter<T>();
            JsonParser.Parse(json, writer);
            return writer.Result;
        }

        private static T Clone<T>(T obj)
        {
            return CopyTo<T>(obj);
        }

        private static T CopyTo<T>(object obj)
        {
            ObjectWriter<T> writer = new ObjectWriter<T>();
            ObjectReader.Read(obj, writer, new ObjectParsingOptions { SerializeAllTypes = true, SerializeTypeInformation = TypeInformationLevel.Minimal });
            return writer.Result;
        }
    }
}
