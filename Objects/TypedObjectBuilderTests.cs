using System.Collections.Generic;
using System.Linq;
using json.JsonObjects;
using NUnit.Framework;

namespace json.Objects
{
    [TestFixture]
    public class TypedObjectBuilderTests
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
            Assert.AreEqual(5, Clone((object)5));
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

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, Clone(obj).Array.Select(i => i.Integer).ToList());
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
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, foo.Dictionary["foo"]);
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
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToObject_ThrowsException()
        {
            ParseObject obj = TypedObjectBuilder.GenericInstance.CreateObject();
            obj.SetType(typeof(IntPropertyClass).AssemblyQualifiedName, null);
            obj.AddObject("Integer", new TestParseObject());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToArray_ThrowsException()
        {
            ParseArray array = TypedObjectBuilder.GenericInstance.CreateArray();
            array.AddObject(new TestParseObject());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToObject_ThrowsException()
        {
            ParseObject obj = TypedObjectBuilder.GenericInstance.CreateObject();
            obj.SetType(typeof(SettableListPropertyClass).AssemblyQualifiedName, null);
            obj.AddArray("Array", new TestParseArray());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToArray_ThrowsException()
        {
            ParseArray array = TypedObjectBuilder.GenericInstance.CreateArray();
            array.AddArray(new TestParseArray());
        }

        [Test]
        public void PreDeserializeUpgrade()
        {
            string json = Parse.From.Object(new PreDeserializeUpgradeClass { One = 1, Two = 2 }).ToTypedJson();
            PreDeserializeUpgradeClass obj = Parse.From.Json(json).ToObject<PreDeserializeUpgradeClass>();

            Assert.AreEqual(2, obj.One);
            Assert.AreEqual(1, obj.Two);
        }

        private class PreDeserializeUpgradeClass
        {
            public int One { get; set; }
            public int Two { get; set; }

            [PreDeserialize]
            public JsonObject PreDeserialize(JsonObject json)
            {
                int one = (int)((double?)json.Get("One") ?? 0);
                int two = (int)((double?)json.Get("Two") ?? 0);
                json["One"] = two;
                json["Two"] = one;
                return json;
            }
        }

        [Test]
        public void MaintainObjectReferences()
        {
            IntPropertyClass intProperty = new IntPropertyClass { Integer = 5 };
            SameReferenceTwice clone = Clone(new SameReferenceTwice(intProperty));

            Assert.AreSame(clone.One, clone.Two);
            Assert.AreNotSame(intProperty, clone.One);
        }

        private static T Clone<T>(T obj)
        {
            return Parse.From.Object(obj).ToObject<T>();
        }

        [Test]
        public void ParseToTypedObject()
        {
            IntPropertyClass obj = Parse.From.Json(@"{""Integer"":3}").ToObject<IntPropertyClass>();

            Assert.AreEqual(3, obj.Integer);
        }

        [Test]
        public void ParsePropertyToTypedObject()
        {
            ObjectPropertyClass obj = Parse.From.Json(@"{""Object"":{""Integer"":4}}").ToObject<ObjectPropertyClass>();

            Assert.AreEqual(4, obj.Object.Integer);
        }

        [Test]
        public void ParsePropertyToTypedArray()
        {
            ObjectArrayPropertyClass obj = Parse.From
                .Json(@"{""Array"":[{""Integer"":1},{""Integer"":2},{""Integer"":3}]}")
                .ToObject<ObjectArrayPropertyClass>();

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.Array.Select(i => i.Integer));
        }

        [Test]
        public void ParsePropertyToTypedNestedArray()
        {
            NestedArrayPropertyClass obj = Parse.From
                .Json(@"{""NestedArray"":[[1,2,3],[4,5,6]]}")
                .ToObject<NestedArrayPropertyClass>();

            Assert.NotNull(obj.NestedArray);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, obj.NestedArray[0]);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, obj.NestedArray[1]);
        }
    }
}
