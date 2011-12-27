using System;

namespace json.Objects
{
    // TODO check if this class is actually necessary
    public abstract class JsonArrayDefinition : EnumerableDefinition
    {
        protected JsonArrayDefinition(Type type, Type itemType)
            : base(type, itemType)
        { }

        public abstract void AddToCollection(object collection, object item);
    }
}