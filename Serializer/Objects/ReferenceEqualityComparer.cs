using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace json.Objects
{
    internal class ReferenceEqualityComparer<T> : EqualityComparer<T>
    {
        public override bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}