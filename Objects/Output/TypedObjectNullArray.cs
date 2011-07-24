using System.Collections;

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

        public IEnumerable GetTypedArray()
        {
            yield break;
        }

        public void AddItem(object item)
        {
        }

        public void PopulateCollection(object collection)
        {
        }
    }
}
