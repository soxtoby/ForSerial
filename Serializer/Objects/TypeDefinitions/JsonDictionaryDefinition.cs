﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace json.Objects
{
    public class JsonDictionaryDefinition : TypeDefinition
    {
        private JsonDictionaryDefinition(Type dictionaryType) : base(dictionaryType) { }

        internal static JsonDictionaryDefinition CreateDictionaryDefinition(Type type)
        {
            Type keyType = type.GetGenericInterfaceType(typeof(IDictionary<,>));
            return IsJsonCompatibleKeyType(keyType)
                ? new JsonDictionaryDefinition(type)
                : null;
        }

        private static bool IsJsonCompatibleKeyType(Type keyType)
        {
            TypeCodeType typeCodeType = keyType.GetTypeCodeType();
            return typeCodeType == TypeCodeType.String
                || typeCodeType == TypeCodeType.Number;
        }

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            IDictionary dictionary = input as IDictionary;
            if (dictionary == null) return null;

            OutputStructure output = valueFactory.CreateStructure(input);

            Type valueType = Type.GetGenericInterfaceType(typeof(IDictionary<,>), 1);
            TypeDefinition valueTypeDef = CurrentTypeHandler.GetTypeDefinition(valueType);

            foreach (object key in dictionary.Keys)
            {
                // Convert.ToString is in case the keys are numbers, which are represented
                // as strings when used as keys, but can be indexed with numbers in JavaScript
                string name = System.Convert.ToString(key, CultureInfo.InvariantCulture);
                object value = dictionary[key];

                valueFactory.ReadProperty(valueTypeDef, name, value, output);
            }

            return output;
        }

        public override TypedObject CreateStructure()
        {
            return new TypedDictionary(this);
        }
    }
}