using System.Collections;

namespace json.Objects
{
    public interface TypedObjectArray : ParseArray
    {
        IEnumerable GetTypedArray();
        void AddItem(object item);
        void PopulateCollection(object collection);
    }

    internal static class TypedObjectArrayExtensions
    {
        public static TypedObjectArray GetArrayAsTypedObjectArray(this ParseArray value)
        {
            TypedObjectArray arrayValue = value as TypedObjectArray;

            if (arrayValue == null)
                throw new TypedObjectBuilder.UnsupportedParseArray();

            return arrayValue;
        }
    }
}