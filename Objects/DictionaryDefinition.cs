using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace json.Objects
{
    public class DictionaryDefinition : TypeDefinition
    {
        private DictionaryDefinition(Type dictionaryType) : base(dictionaryType) { }

        internal static DictionaryDefinition CreateDictionaryDefinition(Type type)
        {
            Type keyType = type.GetGenericInterfaceType(typeof(IDictionary<,>));
            return IsJsonCompatibleKeyType(keyType)
                ? new DictionaryDefinition(type)
                : null;
        }

        private static bool IsJsonCompatibleKeyType(Type keyType)
        {
            TypeCodeType typeCodeType = keyType.GetTypeCodeType();
            return typeCodeType == TypeCodeType.String
                || typeCodeType == TypeCodeType.Number;
        }

        public override bool PropertyCanBeSerialized(PropertyDefinition property)
        {
            return property.CanGet;
        }

        public override ParseValue GetParseValue(ParseValueFactory valueFactory)
        {
            return valueFactory.CreateObject();
        }

        public override void ParseObject(object input, ParseValue output, ParserValueFactory valueFactory)
        {
            IDictionary dictionary = input as IDictionary;
            ParseObject obj = output as ParseObject;
            if (dictionary == null || obj == null) return;

            foreach (object key in dictionary.Keys)
            {
                // Convert.ToString is in case the keys are numbers, which are represented
                // as strings when used as keys, but can be indexed with numbers in JavaScript
                string name = Convert.ToString(key, CultureInfo.InvariantCulture);
                object value = dictionary[key];

                ParseValue parseValue = valueFactory.ParseProperty(obj, name, value);
                parseValue.AddToObject(obj, name);
            }
        }
    }
}