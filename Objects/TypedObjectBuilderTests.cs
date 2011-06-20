using System.Collections.Generic;
using System.Linq;
using json.Json;
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
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToObject_ThrowsException()
        {
            ParseObject obj = TypedObjectBuilder.Instance.CreateObject();
            obj.SetType(typeof(IntPropertyClass).AssemblyQualifiedName, null);
            obj.AddObject("Integer", new TestParseObject());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToArray_ThrowsException()
        {
            ParseArray array = TypedObjectBuilder.Instance.CreateArray();
            array.AddObject(new TestParseObject());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToObject_ThrowsException()
        {
            ParseObject obj = TypedObjectBuilder.Instance.CreateObject();
            obj.SetType(typeof(SettableListPropertyClass).AssemblyQualifiedName, null);
            obj.AddArray("Array", new TestParseArray());
        }

        [Test]
        [ExpectedException(typeof(TypedObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToArray_ThrowsException()
        {
            ParseArray array = TypedObjectBuilder.Instance.CreateArray();
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

            [PreDeserializeJson]
            public string PreDeserialize(string json)
            {
                Assert.AreEqual("{\"One\":1,\"Two\":2}", json);
                return "{\"One\":2,\"Two\":1}";
            }

            // This is what I'd like to use once I've written a JsonObjectParser
            //            public void PreDeserialize(JsonObject json)
            //            {
            //                int one = (int?)json.Get("One") ?? 0;
            //                int two = (int?)json.Get("Two") ?? 0;
            //                json["One"] = two;
            //                json["Two"] = one;
            //            }
        }

        private static T Clone<T>(T obj)
        {
            return Parse.From.Object(obj).ToObject<T>();
        }
    }
}
