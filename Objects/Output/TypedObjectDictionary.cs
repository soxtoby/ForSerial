using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    internal class TypedObjectDictionary : TypedObjectParseObject
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public TypeDefinition TypeDef { get; private set; }
        private readonly TypeDefinition keyTypeDef;
        private readonly TypeDefinition valueTypeDef;

        public TypedObjectDictionary(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
            keyTypeDef = TypeDefinition.GetTypeDefinition(TypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 0));
            valueTypeDef = TypeDefinition.GetTypeDefinition(TypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 1));
        }

        public void AddProperty(string name, object value)
        {
            dictionary[name] = value;
        }

        public void AddObject(string name, TypedObjectObject value)
        {
            TypedObjectObject objectValue = TypedObjectObject.GetObjectAsTypedObjectObject(value);
            dictionary[name] = objectValue.Object;
        }

        public void AddArray(string name, TypedObjectArray value)
        {
            TypedObjectArray arrayValue = value.GetArrayAsTypedObjectArray();
            dictionary[name] = arrayValue.GetTypedArray();
        }

        public ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value)
        {
            return valueTypeDef.CreateValue(valueFactory, value);
        }

        public ParseObject CreateObject(string name, ParseValueFactory valueFactory)
        {
            return new TypedObjectObject(valueTypeDef);
        }

        public ParseArray CreateArray(string name, ParseValueFactory valueFactory)
        {
            return new TypedObjectTypedArray(valueTypeDef.Type);
        }

        public void AssignToProperty(object owner, PropertyDefinition property)
        {
            if (property.CanSet)
                SetDictionaryProperty(owner, property);
            else
                PopulateDictionaryProperty(owner, property);
        }

        private void SetDictionaryProperty(object owner, PropertyDefinition property)
        {
            property.SetOn(owner, Object);
        }

        private void PopulateDictionaryProperty(object owner, PropertyDefinition property)
        {
            IDictionary typedDictionary = (IDictionary)property.GetFrom(owner);
            PopulateDictionary(typedDictionary);
        }

        private void PopulateDictionary(IDictionary typedDictionary)
        {
            foreach (KeyValuePair<string, object> item in dictionary)
            {
                typedDictionary[keyTypeDef.ConvertToCorrectType(item.Key)] = valueTypeDef.ConvertToCorrectType(item.Value);
            }
        }

        public object Object
        {
            get
            {
                IDictionary typedDictionary = (IDictionary)Activator.CreateInstance(TypeDef.Type);
                PopulateDictionary(typedDictionary);
                return typedDictionary;
            }
        }
    }
}