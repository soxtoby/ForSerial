using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class NullObjectValue : ObjectOutput
    {
        public static readonly NullObjectValue Instance = new NullObjectValue();

        private NullObjectValue() { }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public object GetTypedValue()
        {
            return null;
        }

        public TypeDefinition TypeDef { get { return NullTypeDefinition.Instance; } }
    }
}