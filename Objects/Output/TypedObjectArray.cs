using System;
using System.Collections;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private interface TypedObjectArray : ParseArray
        {
            IEnumerable Array { get; }
            IEnumerable GetTypedArray(Type type);
        }
    }
}
