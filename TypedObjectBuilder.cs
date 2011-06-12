using System;
using System.Collections.Generic;

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

        public static T GetResult<T>(ParseValue value)
        {
            TypedObjectArray array = value as TypedObjectArray;
            if (array != null)
            {
                CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(typeof(T));
                if (collectionDef.IsCollection)
                {
                    return (T)PopulateCollection(typeof(T), array, () => Activator.CreateInstance(typeof(T)));
                }
            }

            TypedObjectObject obj = value.AsObject() as TypedObjectObject;
            if (obj == null)
                throw new InvalidResultObject();

            return (T)obj.Object;
        }

        private static object PopulateCollection(Type collectionType, TypedObjectArray array, Func<object> getCollection)
        {
            CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(collectionType);
            
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
                object collection = PopulateCollection(property.Type, array, () => Activator.CreateInstance(property.Type));

                if (collection != null)
                    property.SetOn(Object, collection);
            }

            private void PopulateArrayProperty(PropertyDefinition property, TypedObjectArray array)
            {
                PopulateCollection(property.Type, array, () => property.GetFrom(Object));
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

        internal class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects of type TypedObjectObject.") { }
        }

        internal class PropertyTypeMismatch : Exception
        {
            public PropertyTypeMismatch(Type objectType, string propertyName, Type expected, Type actual)
                : base("Type mismatch attempting to set property {0}{1}. Property is {2} and value was {3}."
                    .FormatWith(objectType.FullName, propertyName, expected.FullName, actual.FullName))
            { }
        }

        internal class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type TypedObjectArray.") { }
        }

        internal class ObjectNotInitialized : Exception
        {
            public ObjectNotInitialized() : base("Tried to add a property to an uninitialized object. Make sure input contains type information.") { }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a TypedObjectBuilder.") { }
        }
    }
}

