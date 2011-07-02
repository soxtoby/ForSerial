using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class TypedObjectBuilder : ParseValueFactory
    {
        private Type baseType;

        private TypedObjectBuilder() { }

        public TypedObjectBuilder(Type baseType)
        {
            this.baseType = baseType;
        }

        private static TypedObjectBuilder instance;
        public static TypedObjectBuilder GenericInstance
        {
            get { return instance ?? (instance = new TypedObjectBuilder()); }
        }

        public static T GetResult<T>(ParseValue value)
        {
            TypedObjectArray array = value as TypedObjectArray;
            if (array != null)
                return (T)array.GetTypedArray(typeof(T));

            TypedObjectObject obj = value.AsObject() as TypedObjectObject;
            if (obj == null)
                throw new InvalidResultObject();

            return (T)obj.Object;
        }

        public virtual ParseObject CreateObject()
        {
            TypedObjectObject obj = new TypedObjectObject();
            SetTypeOfBaseObject(obj);
            return obj;
        }

        private void SetTypeOfBaseObject(TypedObjectObject obj)
        {
            if (baseType != null)
            {
                obj.SetType(TypeDefinition.GetTypeDefinition(baseType));
                baseType = null; // Only needed for first object
            }
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

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return parseObject;
        }

        private class TypedObjectObject : ParseObjectBase
        {
            private TypedObjectParseObject parseObject;

            public TypedObjectObject()
            {
            }

            public TypedObjectObject(object obj)
            {
                parseObject = new TypedObjectRegularObject(obj);
            }

            public object Object
            {
                get { return parseObject.Object; }
            }

            public override bool SetType(string typeIdentifier, Parser parser)
            {
                TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(typeIdentifier);

                TypeDefinition.PreBuildInfo preBuildInfo = typeDef.GetPreBuildInfo(parser);

                if (preBuildInfo != null)
                {
                    parseObject = PreBuildRegularObject(parser, preBuildInfo, typeDef);
                    return true;
                }

                SetType(typeDef);

                return false;
            }

            public void SetType(TypeDefinition typeDef)
            {
                parseObject = typeDef.IsJsonCompatibleDictionary
                                  ? (TypedObjectParseObject)new TypedObjectDictionary(typeDef)
                                  : new TypedObjectRegularObject(typeDef);
            }

            private static TypedObjectParseObject PreBuildRegularObject(Parser parser, TypeDefinition.PreBuildInfo preBuildInfo, TypeDefinition typeDef)
            {
                TypedObjectRegularObject regularObject = new TypedObjectRegularObject(typeDef);
                regularObject.PreBuild(preBuildInfo, parser);
                return regularObject;
            }

            public override void AddNull(string name)
            {
                AssertObjectInitialized();
                parseObject.AddNull(name);
            }

            public override void AddBoolean(string name, bool value)
            {
                AssertObjectInitialized();
                parseObject.AddBoolean(name, value);
            }

            public override void AddNumber(string name, double value)
            {
                AssertObjectInitialized();
                parseObject.AddNumber(name, value);
            }

            public override void AddString(string name, string value)
            {
                AssertObjectInitialized();
                parseObject.AddString(name, value);
            }

            public override void AddObject(string name, ParseObject value)
            {
                AssertObjectInitialized();
                parseObject.AddObject(name, value);
            }

            public override void AddArray(string name, ParseArray value)
            {
                AssertObjectInitialized();
                parseObject.AddArray(name, value);
            }

            public override ParseObject CreateObject(string name, ParseValueFactory valueFactory)
            {
                AssertObjectInitialized();
                return parseObject.CreateObject(name, valueFactory);
            }

            public override ParseArray CreateArray(string name, ParseValueFactory valueFactory)
            {
                AssertObjectInitialized();
                return parseObject.CreateArray(name, valueFactory);
            }

            private void AssertObjectInitialized()
            {
                if (parseObject == null)
                    throw new ObjectNotInitialized();
            }

            public void AssignToProperty(object owner, PropertyDefinition property)
            {
                parseObject.AssignToProperty(owner, property);
            }
        }

        private interface TypedObjectParseObject : ParseObject
        {
            void AssignToProperty(object owner, PropertyDefinition property);
            object Object { get; }
        }

        private class TypedObjectRegularObject : ParseObjectBase, TypedObjectParseObject
        {
            private readonly TypeDefinition typeDef;
            public object Object { get; private set; }

            public TypedObjectRegularObject(TypeDefinition typeDef)
            {
                this.typeDef = typeDef;
                Object = Activator.CreateInstance(typeDef.Type);
            }

            public TypedObjectRegularObject(object obj)
            {
                Object = obj;
            }

            private class TypedObjectSubBuilder : TypedObjectBuilder
            {
                private readonly TypedObjectRegularObject baseObject;
                private bool isBase = true;

                public TypedObjectSubBuilder(TypedObjectRegularObject baseObject)
                {
                    this.baseObject = baseObject;
                }

                public override ParseObject CreateObject()
                {
                    if (!isBase)
                        return base.CreateObject();

                    isBase = false;
                    return baseObject;
                }
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
                TypedObjectObject objectValue = GetObjectAsTypedObjectObject(value);

                PropertyDefinition property = typeDef.Properties.Get(name);
                if (property != null)
                    objectValue.AssignToProperty(Object, property);
            }

            public override void AddArray(string name, ParseArray value)
            {
                TypedObjectArray array = GetArrayAsTypedObjectArray(value);

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
                property.SetOn(Object, array.GetTypedArray(property.TypeDef.Type));
            }

            private void PopulateArrayProperty(PropertyDefinition property, TypedObjectArray array)
            {
                PopulateCollection(property.TypeDef.Type, array.Array, () => property.GetFrom(Object));
            }

            private void SetProperty(string name, object value)
            {
                PropertyDefinition property = typeDef.Properties.Get(name);
                if (property != null)
                    property.SetOn(Object, value);
            }

            public void AssignToProperty(object owner, PropertyDefinition property)
            {
                if (!property.TypeDef.Type.IsAssignableFrom(typeDef.Type))
                    throw new PropertyTypeMismatch(owner.GetType(), property.Name, property.TypeDef.Type, typeDef.Type);

                property.SetOn(owner, Object);
            }

            public void PreBuild(TypeDefinition.PreBuildInfo preBuildInfo, Parser parser)
            {
                preBuildInfo.PreBuild(Object, parser, ((Func<ParseValueFactory>)(() => new TypedObjectSubBuilder(this)))());
            }

            public override ParseObject CreateObject(string name, ParseValueFactory valueFactory)
            {
                AssertCorrectValueFactoryType(valueFactory);

                TypedObjectObject obj = (TypedObjectObject)valueFactory.CreateObject();
                PropertyDefinition property = typeDef.Properties.Get(name);
                if (property != null)
                    obj.SetType(property.TypeDef);
                return obj;
            }

            public override ParseArray CreateArray(string name, ParseValueFactory valueFactory)
            {
                AssertCorrectValueFactoryType(valueFactory);

                TypedObjectArray array = (TypedObjectArray)valueFactory.CreateArray();
                PropertyDefinition property = typeDef.Properties.Get(name);
                if (property != null)
                    array.SetType(property.TypeDef);
                return array;
            }
        }

        private class TypedObjectDictionary : ParseObjectBase, TypedObjectParseObject
        {
            private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
            private readonly TypeDefinition dictionaryTypeDef;
            private readonly TypeDefinition keyTypeDef;
            private readonly TypeDefinition valueTypeDef;

            public TypedObjectDictionary(TypeDefinition typeDef)
            {
                dictionaryTypeDef = typeDef;
                keyTypeDef = TypeDefinition.GetTypeDefinition(dictionaryTypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 0));
                valueTypeDef = TypeDefinition.GetTypeDefinition(dictionaryTypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 1));
            }

            public override void AddNull(string name)
            {
                dictionary[name] = null;
            }

            public override void AddBoolean(string name, bool value)
            {
                dictionary[name] = value;
            }

            public override void AddNumber(string name, double value)
            {
                dictionary[name] = value;
            }

            public override void AddString(string name, string value)
            {
                dictionary[name] = value;
            }

            public override void AddObject(string name, ParseObject value)
            {
                TypedObjectObject objectValue = GetObjectAsTypedObjectObject(value);
                dictionary[name] = objectValue.Object;
            }

            public override void AddArray(string name, ParseArray value)
            {
                TypedObjectArray arrayValue = GetArrayAsTypedObjectArray(value);
                dictionary[name] = arrayValue.GetTypedArray(valueTypeDef.Type);
            }

            public void AssignToProperty(object owner, PropertyDefinition property)
            {
                if (property.CanSet)
                    SetDictionaryProperty(owner, property);
                else
                    PopulateDictionaryProperty(owner, property);
            }

            private void SetDictionaryProperty(object owner, PropertyDefinition property)
            {
                property.SetOn(owner, Object);
            }

            private void PopulateDictionaryProperty(object owner, PropertyDefinition property)
            {
                IDictionary typedDictionary = (IDictionary)property.GetFrom(owner);
                PopulateDictionary(typedDictionary);
            }

            private void PopulateDictionary(IDictionary typedDictionary)
            {
                foreach (KeyValuePair<string, object> item in dictionary)
                {
                    typedDictionary[keyTypeDef.ConvertToCorrectType(item.Key)] = valueTypeDef.ConvertToCorrectType(item.Value);
                }
            }

            public object Object
            {
                get
                {
                    IDictionary typedDictionary = (IDictionary)Activator.CreateInstance(dictionaryTypeDef.Type);
                    PopulateDictionary(typedDictionary);
                    return typedDictionary;
                }
            }
        }

        private static TypedObjectObject GetObjectAsTypedObjectObject(ParseObject value)
        {
            TypedObjectObject objectValue = value as TypedObjectObject;

            if (objectValue == null)
                throw new UnsupportedParseObject();

            return objectValue;
        }

        private static TypedObjectArray GetArrayAsTypedObjectArray(ParseArray value)
        {
            TypedObjectArray arrayValue = value as TypedObjectArray;

            if (arrayValue == null)
                throw new UnsupportedParseArray();

            return arrayValue;
        }

        private class TypedObjectArray : ParseArrayBase
        {
            private TypedObjectParseArray parseArray = new TypedObjectUnknownTypeArray();

            public void SetType(TypeDefinition typeDef)
            {
                parseArray = new TypedObjectTypedArray(typeDef.Type, parseArray);
            }

            public IEnumerable Array
            {
                get { return parseArray.Array; }
            }

            public override ParseObject AsObject()
            {
                return parseArray.AsObject();
            }

            public override void AddNull()
            {
                parseArray.AddNull();
            }

            public override void AddBoolean(bool value)
            {
                parseArray.AddBoolean(value);
            }

            public override void AddNumber(double value)
            {
                parseArray.AddNumber(value);
            }

            public override void AddString(string value)
            {
                parseArray.AddString(value);
            }

            public override void AddObject(ParseObject value)
            {
                parseArray.AddObject(value);
            }

            public override void AddArray(ParseArray value)
            {
                parseArray.AddArray(value);
            }

            public override ParseObject CreateObject(ParseValueFactory valueFactory)
            {
                return parseArray.CreateObject(valueFactory);
            }

            public override ParseArray CreateArray(ParseValueFactory valueFactory)
            {
                return parseArray.CreateArray(valueFactory);
            }

            public object GetTypedArray(Type type)
            {
                return parseArray.GetTypedArray(type);
            }
        }

        private interface TypedObjectParseArray : ParseArray
        {
            IEnumerable Array { get; }
            IEnumerable GetTypedArray(Type type);
        }

        private class TypedObjectUnknownTypeArray : ParseArrayBase, TypedObjectParseArray
        {
            private readonly List<object> objectArray = new List<object>();

            public IEnumerable Array
            {
                get { return objectArray; }
            }

            public IEnumerable GetTypedArray(Type type)
            {
                return PopulateCollection(type, objectArray, () => Activator.CreateInstance(type));
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(objectArray);
            }

            public override void AddNull()
            {
                objectArray.Add(null);
            }

            public override void AddBoolean(bool value)
            {
                objectArray.Add(value);
            }

            public override void AddNumber(double value)
            {
                objectArray.Add(value);
            }

            public override void AddString(string value)
            {
                objectArray.Add(value);
            }

            public override void AddObject(ParseObject value)
            {
                TypedObjectObject obj = GetObjectAsTypedObjectObject(value);
                objectArray.Add(obj.Object);
            }

            public override void AddArray(ParseArray value)
            {
                TypedObjectArray array = GetArrayAsTypedObjectArray(value);
                objectArray.Add(array.Array);
            }
        }

        private class TypedObjectTypedArray : ParseArrayBase, TypedObjectParseArray
        {
            private readonly CollectionDefinition collectionDef;
            public IEnumerable Array { get; private set; }

            public TypedObjectTypedArray(Type collectionType, TypedObjectParseArray untypedArray)
            {
                collectionDef = PopulateCollectionDefinition(collectionType);
                CreateTypedArray(collectionType, untypedArray);
            }

            private static CollectionDefinition PopulateCollectionDefinition(Type collectionType)
            {
                CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(collectionType);
                if (!collectionDef.IsCollection)
                    throw new InvalidCollectionType(collectionType);
                return collectionDef;
            }

            private void CreateTypedArray(Type collectionType, TypedObjectParseArray untypedArray)
            {
                Array = (IEnumerable)Activator.CreateInstance(collectionType);
                foreach (object item in untypedArray.GetTypedArray(collectionType))
                    AddItem(item);
            }

            public IEnumerable GetTypedArray(Type type)
            {
                return Array;
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(Array);
            }

            public override void AddNull()
            {
                AddItem(null);
            }

            public override void AddBoolean(bool value)
            {
                AddItem(value);
            }

            public override void AddNumber(double value)
            {
                AddItem(value);
            }

            public override void AddString(string value)
            {
                AddItem(value);
            }

            public override void AddObject(ParseObject value)
            {
                TypedObjectObject obj = GetObjectAsTypedObjectObject(value);
                AddItem(obj.Object);
            }

            public override void AddArray(ParseArray value)
            {
                TypedObjectArray array = GetArrayAsTypedObjectArray(value);
                AddItem(array.GetTypedArray(collectionDef.ItemTypeDef.Type));
            }

            private void AddItem(object item)
            {
                collectionDef.AddToCollection(Array, item);
            }

            public override ParseObject CreateObject(ParseValueFactory valueFactory)
            {
                AssertCorrectValueFactoryType(valueFactory);

                TypedObjectObject obj = (TypedObjectObject)valueFactory.CreateObject();
                obj.SetType(collectionDef.ItemTypeDef);
                return obj;
            }

            public override ParseArray CreateArray(ParseValueFactory valueFactory)
            {
                AssertCorrectValueFactoryType(valueFactory);

                TypedObjectArray array = (TypedObjectArray)valueFactory.CreateArray();
                array.SetType(collectionDef.ItemTypeDef);
                return array;
            }
        }

        private class TypedObjectNull : ParseNull
        {
            private TypedObjectNull() { }

            private static TypedObjectNull value;
            public static TypedObjectNull Value
            {
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
            public static TypedObjectBoolean True
            {
                get { return trueValue = trueValue ?? new TypedObjectBoolean(true); }
            }

            private static TypedObjectBoolean falseValue;
            public static TypedObjectBoolean False
            {
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

        private static IEnumerable PopulateCollection(Type collectionType, IEnumerable items, Func<object> getCollection)
        {
            CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(collectionType);

            if (collectionDef.IsCollection)
            {
                IEnumerable collection = getCollection() as IEnumerable;

                if (collection != null)
                {
                    foreach (object item in items)
                    {
                        object itemToAdd = TypeInnerCollection(collectionDef.ItemTypeDef.Type, item);
                        collectionDef.AddToCollection(collection, itemToAdd);
                    }
                }

                return collection;
            }

            return null;
        }

        private static object TypeInnerCollection(Type itemType, object item)
        {
            CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(itemType);
            if (collectionDef.IsCollection)
            {
                List<object> innerCollection = item as List<object>;

                if (innerCollection == null)
                    throw new ExpectedCollection(item.GetType());

                return PopulateCollection(itemType, innerCollection, () => Activator.CreateInstance(itemType));
            }
            return item;
        }

        private static void AssertCorrectValueFactoryType(ParseValueFactory valueFactory)
        {
            if (!(valueFactory is TypedObjectBuilder))
                throw new UnsupportedValueFactory();
        }

        internal class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects that created by a TypedObjectBuilder.") { }
        }

        internal class PropertyTypeMismatch : Exception
        {
            public PropertyTypeMismatch(Type objectType, string propertyName, Type expected, Type actual)
                : base("Type mismatch attempting to set property {0}.{1}. Property is {2} and value was {3}."
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

        internal class ExpectedCollection : Exception
        {
            public ExpectedCollection(Type actual) : base("Expected inner collection but found {0}.".FormatWith(actual.FullName)) { }
        }

        private class UnsupportedValueFactory : Exception
        {
            public UnsupportedValueFactory() : base("valueFactory must be a TypedObjectBuilder") { }
        }

        private class InvalidCollectionType : Exception
        {
            public InvalidCollectionType(Type type) : base("Cannot create collection of type {0}.".FormatWith(type.FullName)) { }
        }
    }
}

