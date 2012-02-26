using System;

namespace json.Objects
{
    internal class NullObjectValue : ObjectValue
    {
        public static readonly NullObjectValue Instance = new NullObjectValue();

        private NullObjectValue() { }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public object GetTypedValue()
        {
            return null;
        }

        public TypeDefinition TypeDef { get { throw new NotImplementedException(); } }
    }
}