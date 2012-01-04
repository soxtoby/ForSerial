using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class TypedObjectEnumerable : ParseArrayBase, TypedObjectArray
    {
        private readonly TypeDefinition itemTypeDef;
        private readonly IList collection;

        public TypedObjectEnumerable(TypeDefinition itemTypeDef)
        {
            this.itemTypeDef = itemTypeDef;
            Type listType = typeof(List<>).MakeGenericType(itemTypeDef.Type);
            collection = (IList)Activator.CreateInstance(listType);
        }

        public override void AddToObject(ParseObject obj, string name)
        {
            ((TypedObjectObject)obj).AddProperty(name, this);
        }

        public override void AddToArray(ParseArray array)
        {
            array.GetArrayAsTypedObjectArray().AddItem(GetTypedValue());
        }

        public override ParseObject AsObject()
        {
            return new TypedObjectObject(GetTypedValue());
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

        public void AddItem(object item)
        {
            collection.Add(itemTypeDef.ConvertToCorrectType(item));
        }
    }
}