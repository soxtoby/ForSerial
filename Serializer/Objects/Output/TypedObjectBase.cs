using System;

namespace json.Objects
{
    internal abstract class TypedObjectBase : TypedObject
    {
        public TypeDefinition TypeDef { get; protected set; }
        public abstract object Object { get; }
        public abstract void AddProperty(string name, TypedValue value);

        public void AssignToProperty(object owner, PropertyDefinition property)
        {
            if (!TypeDef.Type.CanBeCastTo(property.TypeDef.Type))
                throw new PropertyTypeMismatch(owner.GetType(), property.Name, property.TypeDef.Type, TypeDef.Type);

            property.SetOn(owner, Object);
        }

        public Output CreateValue(string name, object value)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            return property != null
                ? property.TypeDef.CreateValue(value)
                : TypedNull.Value;
        }

        public OutputStructure CreateStructure(string name)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            return property != null
                ? CanCreateNewPropertyInstance(property)
                        ? new TypedObjectOutputStructure(property.TypeDef)
                        : new TypedObjectOutputStructure()
                : new TypedObjectOutputStructure(NullTypedObject.Instance);
        }

        private static bool CanCreateNewPropertyInstance(PropertyDefinition property)
        {
            return property.TypeDef.IsDeserializable;
        }

        public SequenceOutput CreateSequence(string name)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            return property != null
                ? property.TypeDef.CreateSequence()
                : TypedNullArray.Instance;
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