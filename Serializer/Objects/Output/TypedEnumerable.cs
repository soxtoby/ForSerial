using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class TypedEnumerable : SequenceOutputBase, TypedSequence
    {
        private readonly TypeDefinition itemTypeDef;
        private readonly IList collection;

        public TypedEnumerable(TypeDefinition itemTypeDef)
        {
            this.itemTypeDef = itemTypeDef;
            Type listType = typeof(List<>).MakeGenericType(itemTypeDef.Type);
            collection = (IList)Activator.CreateInstance(listType);
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((TypedObjectOutputStructure)structure).AddProperty(name, this);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            sequence.GetArrayAsTypedObjectArray().AddItem(GetTypedValue());
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedValue());
        }

        public object GetTypedValue()
        {
            return collection;
        }

        public TypeDefinition TypeDef { get { throw new NotImplementedException(); } }

        public void AddItem(object item)
        {
            collection.Add(itemTypeDef.ConvertToCorrectType(item));
        }
    }
}