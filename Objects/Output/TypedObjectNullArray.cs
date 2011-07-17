using System;
using System.Collections;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectNullArray : NullParseArray, TypedObjectArray
        {
            public IEnumerable Array
            {
                get { yield break; }
            }

            public IEnumerable GetTypedArray(Type type)
            {
                yield break;
            }
        }
    }
}
