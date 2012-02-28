using System;
using System.Collections.Generic;
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
            Type listType = typeof(List<>).MakeGenericType(enumerableDef.ItemTypeDef.Type);
            return GetTypedValue((CollectionDefinition)CurrentTypeHandler.GetTypeDefinition(listType));
        }
    }
}