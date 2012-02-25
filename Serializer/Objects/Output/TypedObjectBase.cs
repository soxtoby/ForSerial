using System;

namespace json.Objects
{
    internal abstract class TypedObjectBase : TypedObject
    {
        private Writer currentPropertyWriter;
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
            Writer propertyWriter = TypeDef.GetWriterForProperty(name);
            return propertyWriter.CreateValue(value);
        }

        public OutputStructure BeginStructure(string name)
        {
            currentPropertyWriter = TypeDef.GetWriterForProperty(name);
            return currentPropertyWriter.BeginStructure();
        }

        public SequenceOutput BeginSequence(string name)
        {
            currentPropertyWriter = TypeDef.GetWriterForProperty(name);
            return currentPropertyWriter.BeginSequence();
        }

        public void EndStructure()
        {
            currentPropertyWriter.EndStructure();
        }

        public void EndSequence()
        {
            currentPropertyWriter.EndSequence();
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