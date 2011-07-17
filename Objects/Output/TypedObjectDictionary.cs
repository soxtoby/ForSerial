using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectDictionary : ParseObjectBase, TypedObjectParseObject
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

            public override void AddNull(string name)
            {
                dictionary[name] = null;
            }

            public override void AddBoolean(string name, bool value)
            {
                dictionary[name] = value;
            }

            public override void AddNumber(string name, double value)
            {
                dictionary[name] = value;
            }

            public override void AddString(string name, string value)
            {
                dictionary[name] = value;
            }

            public override void AddObject(string name, ParseObject value)
            {
                TypedObjectObject objectValue = GetObjectAsTypedObjectObject(value);
                dictionary[name] = objectValue.Object;
            }

            public override void AddArray(string name, ParseArray value)
            {
                TypedObjectArray arrayValue = GetArrayAsTypedObjectArray(value);
                dictionary[name] = arrayValue.GetTypedArray(valueTypeDef.Type);
            }

            public override ParseObject CreateObject(string name, ParseValueFactory valueFactory)
            {
                return new TypedObjectObject(valueTypeDef);
            }

            public override ParseArray CreateArray(string name, ParseValueFactory valueFactory)
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
}
