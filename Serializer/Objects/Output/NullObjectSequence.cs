using System;
using System.Linq;

namespace json.Objects
{
    internal class NullObjectSequence : ObjectSequence
    {
        public static readonly NullObjectSequence Instance = new NullObjectSequence();

        private NullObjectSequence() { }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public object GetTypedValue()
        {
            return Enumerable.Empty<object>();
        }

        public TypeDefinition TypeDef { get { throw new NotImplementedException(); } }

        public ObjectStructure CreateStructure()
        {
            return NullObjectStructure.Instance;
        }

        public ObjectSequence CreateSequence()
        {
            return Instance;
        }

        public bool CanCreateValue(object value)
        {
            return true;
        }

        public void Add(ObjectOutput value) { }

        public ObjectValue CreateValue(object value)
        {
            return NullObjectValue.Instance;
        }
    }
}