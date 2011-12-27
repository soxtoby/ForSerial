using System;

namespace json.Objects
{
    public class TypedObjectObject : ParseObjectBase, TypedObjectValue
    {
        private TypedObjectParseObject parseObject;

        public TypedObjectObject()
        {
        }

        public TypedObjectObject(object obj)
        {
            parseObject = new TypedObjectRegularObject(obj);
        }

        public TypedObjectObject(TypeDefinition typeDef)
        {
            SetType(typeDef);
        }

        public object Object
        {
            get { return parseObject.Object; }
        }

        public override bool SetType(string typeIdentifier, Parser parser)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(typeIdentifier);
            bool useCurrentType = CurrentTypeIsNotCompatible(typeDef);
            if (useCurrentType)
                typeDef = parseObject.TypeDef;

            PreBuildInfo preBuildInfo = typeDef.GetPreBuildInfo(parser);

            if (preBuildInfo != null)
            {
                parseObject = PreBuildRegularObject(parser, preBuildInfo, typeDef);
                return true;
            }

            if (!useCurrentType)
                SetType(typeDef);

            return false;
        }

        private bool CurrentTypeIsNotCompatible(TypeDefinition typeDef)
        {
            return parseObject != null && !typeDef.Type.CanBeCastTo(parseObject.TypeDef.Type);
        }

        private void SetType(TypeDefinition typeDef)
        {
            parseObject = typeDef.CreateObject();
        }

        private static TypedObjectParseObject PreBuildRegularObject(Parser parser, PreBuildInfo preBuildInfo, TypeDefinition typeDef)
        {
            TypedObjectRegularObject regularObject = new TypedObjectRegularObject(typeDef);
            regularObject.PreBuild(preBuildInfo, parser);
            return regularObject;
        }

        public void AddProperty(string name, TypedObjectValue value)
        {
            AssertObjectInitialized();
            parseObject.AddProperty(name, value);
        }

        public override ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value)
        {
            AssertObjectInitialized();
            return parseObject.CreateValue(name, valueFactory, value);
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

        public object GetTypedValue()
        {
            return Object;
        }

        internal static TypedObjectObject GetObjectAsTypedObjectObject(ParseObject value)
        {
            TypedObjectObject objectValue = value as TypedObjectObject;

            if (objectValue == null)
                throw new UnsupportedParseObject();

            return objectValue;
        }

        internal class ObjectNotInitialized : Exception
        {
            public ObjectNotInitialized() : base("Tried to add a property to an uninitialized object. Make sure input contains type information.") { }
        }

        internal class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects that created by a TypedObjectBuilder.") { }
        }

        public override void AddToObject(ParseObject obj, string name)
        {
            ((TypedObjectObject)obj).AddProperty(name, this);
        }

        public override void AddToArray(ParseArray array)
        {
            ((TypedObjectArray)array).AddItem(parseObject.Object);
        }
    }
}
