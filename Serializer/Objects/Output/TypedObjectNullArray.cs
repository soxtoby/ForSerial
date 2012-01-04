using System.Linq;

namespace json.Objects
{
    internal class TypedObjectNullArray : NullParseArray, TypedObjectArray
    {
        private TypedObjectNullArray() { }

        private static TypedObjectNullArray instance;
        public new static TypedObjectNullArray Instance
        {
            get { return instance ?? (instance = new TypedObjectNullArray()); }
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
