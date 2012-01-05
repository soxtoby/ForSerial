using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    internal class TypedDictionary : TypedObject
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public TypeDefinition TypeDef { get; private set; }
        private readonly TypeDefinition keyTypeDef;
        private readonly TypeDefinition valueTypeDef;

        public TypedDictionary(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
            keyTypeDef = CurrentTypeHandler.GetTypeDefinition(TypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 0));
            valueTypeDef = CurrentTypeHandler.GetTypeDefinition(TypeDef.Type.GetGenericInterfaceType(typeof(IDictionary<,>), 1));
        }

        public void AddProperty(string name, TypedValue value)
        {
            dictionary[name] = value.GetTypedValue();
        }

        public Output CreateValue(string name, object value)
        {
            return valueTypeDef.CreateValue(value);
        }

        public OutputStructure CreateStructure(string name)
        {
            return new TypedObjectOutputStructure(valueTypeDef);
        }

        public SequenceOutput CreateSequence(string name)
        {
            return valueTypeDef.CreateSequence();
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