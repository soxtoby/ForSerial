using System;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    internal class CollectionSequence : BaseObjectSequence
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
                property.SetOn(obj, GetTypedValue());
            else if (property.CanGet)
                PopulateCollection(property.GetFrom(obj));
        }

        public override object GetTypedValue()
        {
            object collection = collectionDef.ConstructNew();
            PopulateCollection(collection);
            return collection;
        }

        private void PopulateCollection(object collection)
        {
            if (collection != null)
                foreach (ObjectOutput value in Items)
                    collectionDef.AddToCollection(collection, value.GetTypedValue());
        }
    }
}