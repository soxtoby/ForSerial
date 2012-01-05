using System.Linq;

namespace json.Objects
{
    internal class TypedNullArray : NullSequence, TypedSequence
    {
        private TypedNullArray() { }

        private static TypedNullArray instance;
        public new static TypedNullArray Instance
        {
            get { return instance ?? (instance = new TypedNullArray()); }
        }

        public void AddItem(object item)
        {
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
        }

        public object GetTypedValue()
        {
            return Enumerable.Empty<object>();
        }
    }
}
