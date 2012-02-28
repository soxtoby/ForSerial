
namespace json.Objects
{
    public class PropertyDefinition
    {
        public PropertyDefinition(TypeDefinition typeDef, string name, GetMethod getter, SetMethod setter, bool forceTypeIdentifierSerialization)
        {
            this.getter = getter;
            this.setter = setter;
            Name = name;
            TypeDef = typeDef;
            this.forceTypeIdentifierSerialization = forceTypeIdentifierSerialization;
        }

        public string Name { get; private set; }
        public TypeDefinition TypeDef { get; private set; }
        private readonly bool forceTypeIdentifierSerialization;

        private readonly GetMethod getter;
        private readonly SetMethod setter;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public object GetFrom(object source)
        {
            return getter(source);
        }

        public void SetOn(object target, object value)
        {
            if (CanSet)
                setter(target, TypeDef.ConvertToCorrectType(value));
        }

        public ObjectContainer CreateStructure()
        {
            return TypeDef.CreateStructure();
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return TypeDef.CreateStructure(typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return TypeDef.CreateSequence();
        }

        public ObjectValue CreateValue(object value)
        {
            return TypeDef.CreateValue(value);
        }

        public bool ShouldWriteTypeIdentifier(object value)
        {
            if (forceTypeIdentifierSerialization)
                return true;

            TypeDefinition valueTypeDef = CurrentTypeHandler.GetTypeDefinition(value);
            return valueTypeDef != TypeDef;
        }
    }
}
