using System.Linq;

namespace json.Objects
{
    internal class NullObjectSequence : ObjectContainer
    {
        public static readonly NullObjectSequence Instance = new NullObjectSequence();

        private NullObjectSequence() { }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public object GetTypedValue()
        {
            return Enumerable.Empty<object>();
        }

        public TypeDefinition TypeDef { get { return NullTypeDefinition.Instance; } }

        public void SetCurrentProperty(string name) { }

        public void SetType(string typeIdentifier) { }

        public ObjectContainer CreateStructure()
        {
            return NullObjectStructure.Instance;
        }

        public ObjectContainer CreateSequence()
        {
            return Instance;
        }

        public bool CanCreateValue(object value)
        {
            return true;
        }

        public void Add(ObjectOutput value) { }

        public void WriteValue(object value) { }
    }
}