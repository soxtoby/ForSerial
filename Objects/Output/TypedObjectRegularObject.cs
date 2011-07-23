using System;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectRegularObject : ParseObjectBase, TypedObjectParseObject
        {
            public TypeDefinition TypeDef { get; private set; }
            public object Object { get; private set; }

            public TypedObjectRegularObject(TypeDefinition typeDef)
            {
                TypeDef = typeDef;
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

                PropertyDefinition property = TypeDef.Properties.Get(name);
                if (property != null)
                    objectValue.AssignToProperty(Object, property);
            }

            public override void AddArray(string name, ParseArray value)
            {
                TypedObjectArray array = GetArrayAsTypedObjectArray(value);

                PropertyDefinition property = TypeDef.Properties.Get(name);

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
                PopulateCollection(property.TypeDef, array.Array, () => property.GetFrom(Object));
            }

            private void SetProperty(string name, object value)
            {
                PropertyDefinition property = TypeDef.Properties.Get(name);
                if (property != null)
                    property.SetOn(Object, value);
            }

            public void AssignToProperty(object owner, PropertyDefinition property)
            {
                if (!TypeDef.Type.CanBeCastTo(property.TypeDef.Type))
                    throw new PropertyTypeMismatch(owner.GetType(), property.Name, property.TypeDef.Type, TypeDef.Type);

                property.SetOn(owner, Object);
            }

            public void PreBuild(PreBuildInfo preBuildInfo, Parser parser)
            {
                preBuildInfo.PreBuild(Object, parser, ((Func<ParseValueFactory>)(() => new TypedObjectSubBuilder(this)))());
            }

            public override ParseObject CreateObject(string name, ParseValueFactory valueFactory)
            {
                PropertyDefinition property = TypeDef.Properties.Get(name);
                return CanCreateNewPropertyInstance(property)
                    ? new TypedObjectObject(property.TypeDef)
                    : new TypedObjectObject();
            }

            private static bool CanCreateNewPropertyInstance(PropertyDefinition property)
            {
                return property != null && property.TypeDef.IsDeserializable;
            }

            public override ParseArray CreateArray(string name, ParseValueFactory valueFactory)
            {
                PropertyDefinition property = TypeDef.Properties.Get(name);
                return property == null
                    ? (ParseArray)new TypedObjectNullArray()    // No property - don't care what's in the array
                    : new TypedObjectTypedArray(property.TypeDef.Type);
            }
        }

        internal class PropertyTypeMismatch : Exception
        {
            public PropertyTypeMismatch(Type objectType, string propertyName, Type expected, Type actual)
                : base("Type mismatch attempting to set property {0}.{1}. Property is {2} and value was {3}."
                    .FormatWith(objectType.FullName, propertyName, expected.FullName, actual.FullName))
            { }
        }
    }
}
