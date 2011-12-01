using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class DictionaryDefinition : JsonArrayDefinition
    {
        private readonly TypeDefinition keyTypeDef;
        private readonly TypeDefinition valueTypeDef;

        private DictionaryDefinition(Type dictionaryType)
            : base(dictionaryType, typeof(KeyValuePair<object, object>))
        {
            keyTypeDef = CurrentTypeHandler.GetTypeDefinition(dictionaryType.GetGenericInterfaceType(typeof(IDictionary<,>), 0));
            valueTypeDef = CurrentTypeHandler.GetTypeDefinition(dictionaryType.GetGenericInterfaceType(typeof(IDictionary<,>), 1));
        }

        internal static DictionaryDefinition CreateDictionaryDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(IDictionary))
                ? new DictionaryDefinition(type)
                : null;
        }

        public override void AddToCollection(object collection, object item)
        {
            KeyValuePair<object, object> keyValuePair = (KeyValuePair<object, object>)item;

            object key = keyTypeDef != null
                ? keyTypeDef.ConvertToCorrectType(keyValuePair.Key)
                : keyValuePair.Key;
            object value = valueTypeDef != null
                ? valueTypeDef.ConvertToCorrectType(keyValuePair.Value)
                : keyValuePair.Value;

            ((IDictionary)collection).Add(key, value);
        }
    }
}