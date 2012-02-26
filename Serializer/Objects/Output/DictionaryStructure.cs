using System.Collections;
using System.Collections.Generic;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public class DictionaryStructure : BaseObjectStructure
    {
        private readonly JsonDictionaryDefinition dictionaryDef;

        public DictionaryStructure(JsonDictionaryDefinition dictionaryDefinition)
            : base(dictionaryDefinition)
        {
            this.dictionaryDef = dictionaryDefinition;
        }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedValue());
            else if (property.CanGet)
                PopulateDictionary(property.GetFrom(obj));
        }

        public override object GetTypedValue()
        {
            object dictionary = dictionaryDef.ConstructNew();
            PopulateDictionary(dictionary);
            return dictionary;
        }

        private void PopulateDictionary(object dictionary)
        {
            IDictionary typedDictionary = dictionary as IDictionary;
            if (typedDictionary == null) return;

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
                typedDictionary[dictionaryDef.KeyTypeDef.ConvertToCorrectType(property.Key)] = dictionaryDef.ValueTypeDef.ConvertToCorrectType(property.Value.GetTypedValue());
        }
    }
}