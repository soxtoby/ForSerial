using System;

namespace json.Objects
{
    public abstract class JsonArrayDefinition : EnumerableDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }

        protected JsonArrayDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
        }

        public abstract void AddToCollection(object collection, object item);
    }
}