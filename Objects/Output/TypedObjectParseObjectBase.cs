using System;

namespace json.Objects
{
    internal abstract class TypedObjectParseObjectBase : TypedObjectParseObject
    {
        public TypeDefinition TypeDef { get; protected set; }
        public abstract object Object { get; }
        public abstract void AddProperty(string name, object value);
        public abstract void AddObject(string name, TypedObjectObject value);
        public abstract void AddArray(string name, TypedObjectArray array);

        public void AssignToProperty(object owner, PropertyDefinition property)
        {
            if (!TypeDef.Type.CanBeCastTo(property.TypeDef.Type))
                throw new PropertyTypeMismatch(owner.GetType(), property.Name, property.TypeDef.Type, TypeDef.Type);

            property.SetOn(owner, Object);
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