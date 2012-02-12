
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
            ForceTypeIdentifierSerialization = forceTypeIdentifierSerialization;
        }

        public string Name { get; private set; }
        public TypeDefinition TypeDef { get; private set; }
        public bool ForceTypeIdentifierSerialization { get; private set; }

        private readonly GetMethod getter;
        private readonly SetMethod setter;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public Writer Writer
        {
            get { return new TypedObjectBuilder(TypeDef.Type); }
        }

        public object GetFrom(object source)
        {
            return getter(source);
        }

        public void SetOn(object target, object value)
        {
            if (CanSet)
                setter(target, TypeDef.ConvertToCorrectType(value));
        }
    }
}
