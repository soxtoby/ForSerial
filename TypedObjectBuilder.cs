using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
namespace json
{
    public class TypedObjectBuilder : ParseValueFactory
    {
        public static object GetResult(ParseObject obj)
        {
            TypedObjectObject objectObj = obj as TypedObjectObject;
            if (objectObj == null)
                throw new InvalidResultObject();
            return objectObj.Object;
        }

        public ParseObject CreateObject()
        {
            return new TypedObjectObject();
        }

        public ParseArray CreateArray()
        {
            return new TypedObjectArray();
        }

        public ParseNumber CreateNumber(double value)
        {
            return new TypedObjectNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return new TypedObjectString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return value ? TypedObjectBoolean.True : TypedObjectBoolean.False;
        }

        public ParseNull CreateNull()
        {
            return TypedObjectNull.Value;
        }

        private class TypedObjectObject : ParseObjectBase
        {
            private TypeDefinition typeDef;
            public object Object { get; private set; }

            public TypedObjectObject()
            {
            }

            public TypedObjectObject(object obj)
            {
                Object = obj;
            }

            public override void SetTypeIdentifier(string typeIdentifier)
            {
                typeDef = TypeDefinition.GetTypeDefinition(typeIdentifier);
                Object = Activator.CreateInstance(typeDef.Type);
            }

            public override void AddNull(string name)
            {
                SetProperty(name, null);
            }

            public override void AddBoolean(string name, bool value)
            {
                SetProperty(name, value);
            }

            public override void AddNumber(string name, double value)
            {
                SetProperty(name, value);
            }

            public override void AddString(string name, string value)
            {
                SetProperty(name, value);
            }

            public override void AddObject(string name, ParseObject value)
            {
                TypedObjectObject objectValue = value as TypedObjectObject;
                if (objectValue == null)
                    throw new UnsupportedParseObject();

                PropertyDefinition property = typeDef.Properties.Get(name);

                if (property != null)
                {
                    if (!property.Type.IsAssignableFrom(objectValue.typeDef.Type))
                        throw new PropertyTypeMismatch(typeDef.Type, name, property.Type, objectValue.typeDef.Type);

                    property.SetOn(Object, objectValue.Object);
                }
            }

            public override void AddArray(string name, ParseArray value)
            {
                TypedObjectArray array = value as TypedObjectArray;

                if (array == null)
                    throw new UnsupportedParseArray();

                PropertyDefinition property = typeDef.Properties.Get(name);

                if (property != null)
                {
                    if (property.CanSet)
                        SetArrayProperty(property, array);
                    else if (property.CanGet)
                        PopulateArrayProperty(property, array);
                }
            }

            private void SetArrayProperty(PropertyDefinition property, TypedObjectArray array)
            {
                object collection = PopulateArray(property, array, () => Activator.CreateInstance(property.Type));

                if (collection != null)
                    property.SetOn(Object, collection);
            }

            private void PopulateArrayProperty(PropertyDefinition property, TypedObjectArray array)
            {
                PopulateArray(property, array, () => property.GetFrom(Object));
            }

            private static object PopulateArray(PropertyDefinition property, TypedObjectArray array, Func<object> getCollection)
            {
                CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(property.Type);

                if (collectionDef != null && collectionDef.IsCollection)
                {
                    object collection = getCollection();

                    if (collection != null)
                    {
                        foreach (object item in array.Array)
                            collectionDef.AddToCollection(collection, item);
                    }

                    return collection;
                }

                return null;
            }

            private void SetProperty(string name, object value)
            {
                PropertyDefinition property = typeDef.Properties.Get(name);
                if (property != null)
                {
                    property.SetOn(Object, value);
                }
            }
        }

        private class TypedObjectArray : ParseArrayBase
        {
            public IList<object> Array { get; private set; }

            public TypedObjectArray()
            {
                Array = new List<object>();
            }

            public override void AddNull()
            {
                Array.Add(null);
            }

            public override void AddBoolean(bool value)
            {
                Array.Add(value);
            }

            public override void AddNumber(double value)
            {
                Array.Add(value);
            }

            public override void AddString(string value)
            {
                Array.Add(value);
            }

            public override void AddObject(ParseObject value)
            {
                TypedObjectObject obj = value as TypedObjectObject;

                if (obj == null)
                    throw new UnsupportedParseObject();

                Array.Add(obj.Object);
            }

            public override void AddArray(ParseArray value)
            {
                TypedObjectArray array = value as TypedObjectArray;

                if (array == null)
                    throw new UnsupportedParseArray();

                Array.Add(array.Array);
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(Array);
            }
        }

        private class TypedObjectNull : ParseNull
        {
            private TypedObjectNull() { }

            private static TypedObjectNull value;
            public static TypedObjectNull Value {
                get { return value = value ?? new TypedObjectNull(); }
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(null);
            }
        }

        private class TypedObjectBoolean : ParseBoolean
        {
            private TypedObjectBoolean(bool value) : base(value) { }

            private static TypedObjectBoolean trueValue;
            public static TypedObjectBoolean True {
                get { return trueValue = trueValue ?? new TypedObjectBoolean(true); }
            }

            private static TypedObjectBoolean falseValue;
            public static TypedObjectBoolean False {
                get { return falseValue = falseValue ?? new TypedObjectBoolean(false); }
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }
        }

        private class TypedObjectNumber : ParseNumber
        {
            public TypedObjectNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }
        }

        private class TypedObjectString : ParseString
        {
            public TypedObjectString(string value) : base(value) { }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(value);
            }
        }

        private class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects of type TypedObjectObject.") { }
        }

        public class PropertyTypeMismatch : Exception
        {
            public PropertyTypeMismatch(Type objectType, string propertyName, Type expected, Type actual)
                : base("Type mismatch attempting to set property {0}{1}. Property is {2} and value was {3}."
                    .FormatWith(objectType.FullName, propertyName, expected.FullName, actual.FullName))
            { }
        }

        private class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type TypedObjectArray.") { }
        }

        private class ObjectNotInitialized : Exception
        {
            public ObjectNotInitialized() : base("Tried to add a property to an uninitialized object. Make sure input contains type information.") { }
        }

        private class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a TypedObjectBuilder.") { }
        }
    }

    [TestFixture]
    public class TypedObjectBuilderTests
    {
        [Test]
        public void Null()
        {
            Assert.IsNull(Clone(null));
        }

        [Test]
        public void Boolean()
        {
            Assert.IsTrue((bool)Clone(true));
            Assert.IsFalse((bool)Clone(false));
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
            Assert.AreEqual(5, Clone(new ObjectPropertyClass { Object = new IntPropertyClass { Integer = 5} }).Object.Integer);
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
            private List<int> list = new List<int>();
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

            CollectionAssert.AreEqual(new[] { 1, 2, 3}, Clone(obj).Array.Select(i => i.Integer).ToList());
        }

        private class ObjectArrayPropertyClass
        {
            public List<IntPropertyClass> Array { get; set; }
        }

        private T Clone<T>(T obj)
        {
            object clone = Clone((object)obj);
            Assert.IsInstanceOfType(typeof(T), clone);
            return (T)clone;
        }

        private object Clone(object obj)
        {
            ParseObject parseObject = ObjectParser.Parse(obj, new TypedObjectBuilder());
            return TypedObjectBuilder.GetResult(parseObject);
        }
    }
}

