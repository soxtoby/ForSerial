using System.Collections;

namespace json.Objects
{
    public interface TypedObjectArray : ParseArray, TypedObjectValue
    {
        IEnumerable GetTypedArray(); // TODO replace with GetTypedValue
        void AddItem(object item);
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