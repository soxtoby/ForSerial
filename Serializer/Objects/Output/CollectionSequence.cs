using System;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class CollectionSequence : BaseObjectSequence
    {
        private readonly CollectionDefinition collectionDef;

        public CollectionSequence(CollectionDefinition collectionDefinition)
            : base(collectionDefinition)
        {
            if (collectionDefinition == null) throw new ArgumentNullException("collectionDefinition");

            collectionDef = collectionDefinition;
        }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedValue(collectionDef));
            else if (property.CanGet)
                PopulateCollection(collectionDef, property.GetFrom(obj));
        }

        public override object GetTypedValue()
        {
            return GetTypedValue(collectionDef);
        }
    }
}