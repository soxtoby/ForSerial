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
            array.GetArrayAsTypedObjectArray().AddItem(GetTypedArray());
        }

        public override ParseObject AsObject()
        {
            return new TypedObjectObject(GetTypedArray());
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedArray());
        }

        public object GetTypedValue()
        {
            return GetTypedArray();
        }

        public IEnumerable GetTypedArray()
        {
            return collection;
        }

        public void AddItem(object item)
        {
            collection.Add(itemTypeDef.ConvertToCorrectType(item));
        }
    }
}