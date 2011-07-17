using System;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
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
                TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(typeIdentifier);
                bool useCurrentType = CurrentTypeIsNotCompatible(typeDef);
                if (useCurrentType)
                    typeDef = parseObject.TypeDef;

                TypeDefinition.PreBuildInfo preBuildInfo = typeDef.GetPreBuildInfo(parser);

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

        internal class ObjectNotInitialized : Exception
        {
            public ObjectNotInitialized() : base("Tried to add a property to an uninitialized object. Make sure input contains type information.") { }
        }
    }
}
