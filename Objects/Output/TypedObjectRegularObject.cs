using System;

namespace json.Objects
{
    internal class TypedObjectRegularObject : TypedObjectParseObject
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
            if (obj != null)
            {
                TypeDef = TypeDefinition.GetTypeDefinition(obj.GetType());
                Object = obj;
            }
        }

        public void AddObject(string name, TypedObjectObject value)
        {
            TypedObjectObject objectValue = TypedObjectObject.GetObjectAsTypedObjectObject(value);

            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
                objectValue.AssignToProperty(Object, property);
        }

        public void AddArray(string name, TypedObjectArray array)
        {
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
            property.SetOn(Object, array.GetTypedArray());
        }

        private void PopulateArrayProperty(PropertyDefinition property, TypedObjectArray array)
        {
            array.PopulateCollection(property.GetFrom(Object));
        }

        public void AddProperty(string name, object value)
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
            preBuildInfo.PreBuild(Object, parser, new TypedObjectBuilder.TypedObjectSubBuilder(Object));
        }

        public ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            return property == null
                ? valueFactory.CreateValue(value)
                : property.TypeDef.CreateValue(valueFactory, value);
        }

        public ParseObject CreateObject(string name, ParseValueFactory valueFactory)
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

        public ParseArray CreateArray(string name, ParseValueFactory valueFactory)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            return property == null
                ? (ParseArray)TypedObjectNullArray.Instance     // No property - don't care what's in the array
                : new TypedObjectTypedArray(property.TypeDef.Type);
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
