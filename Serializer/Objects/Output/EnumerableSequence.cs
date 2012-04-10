using System;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public class EnumerableSequence : BaseObjectSequence
    {
        private readonly EnumerableDefinition enumerableDef;

        public EnumerableSequence(EnumerableDefinition enumerableDef)
            : base(enumerableDef)
        {
            this.enumerableDef = enumerableDef;
        }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            Type listType = enumerableDef.GetGenericListType();
            return GetTypedValue((CollectionDefinition)TypeCache.GetTypeDefinition(listType));
        }
    }
}