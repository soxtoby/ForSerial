using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace json.Objects.TypeDefinitions
{
    public class JsonDictionaryDefinition : StructureDefinition
    {
        protected JsonDictionaryDefinition(Type dictionaryType)
            : base(dictionaryType)
        {
            KeyTypeDef = TypeCache.GetTypeDefinition(dictionaryType.GetGenericInterfaceType(typeof(IDictionary<,>), 0));
            ValueTypeDef = TypeCache.GetTypeDefinition(dictionaryType.GetGenericInterfaceType(typeof(IDictionary<,>), 1));
        }

        public TypeDefinition ValueTypeDef { get; set; }
        public TypeDefinition KeyTypeDef { get; set; }

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

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            IDictionary dictionary = input as IDictionary;
            if (dictionary == null) return;

            if (reader.ReferenceStructure(input))
                return;

            if (reader.ShouldWriteTypeIdentification(requestTypeIdentification))
                writer.BeginStructure(CurrentTypeResolver.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            foreach (object key in dictionary.Keys)
            {
                // Convert.ToString is in case the keys are numbers, which are represented
                // as strings when used as keys, but can be indexed with numbers in JavaScript
                string name = Convert.ToString(key, CultureInfo.InvariantCulture);
                object value = dictionary[key];

                writer.AddProperty(name);
                ValueTypeDef.ReadObject(value, reader, writer, false);
            }

            writer.EndStructure();
        }

        public override ObjectContainer CreateStructure()
        {
            return new DictionaryStructure(this);
        }

        public override ObjectContainer CreateStructureForProperty(string name)
        {
            return ValueTypeDef.CreateStructure();
        }

        public override ObjectContainer CreateSequenceForProperty(string name)
        {
            return ValueTypeDef.CreateSequence();
        }

        public override ObjectOutput CreateValueForProperty(string name, object value)
        {
            return ValueTypeDef.CreateValue(value);
        }

        public object ConstructNew()
        {
            return ConstructDefault();
        }
    }
}
