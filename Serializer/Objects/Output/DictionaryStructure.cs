using System.Collections;
using System.Collections.Generic;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class DictionaryStructure : BaseObjectStructure
    {
        private readonly JsonDictionaryDefinition dictionaryDef;
        private IDictionary typedDictionary;

        public DictionaryStructure(JsonDictionaryDefinition dictionaryDefinition)
            : base(dictionaryDefinition)
        {
            dictionaryDef = dictionaryDefinition;
        }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
            {
                property.SetOn(obj, GetTypedValue());
            }
            else if (property.CanGet)
            {
                typedDictionary = property.GetFrom(obj) as IDictionary;
                PopulateDictionary();
            }
        }

        public override object GetTypedValue()
        {
            if (typedDictionary != null)
                return typedDictionary;

            typedDictionary = dictionaryDef.ConstructNew() as IDictionary;
            PopulateDictionary();
            return typedDictionary;
        }

        private void PopulateDictionary()
        {
            if (typedDictionary == null)
                return;

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
                typedDictionary[dictionaryDef.KeyTypeDef.ConvertToCorrectType(property.Key)] = dictionaryDef.ValueTypeDef.ConvertToCorrectType(property.Value.GetTypedValue());
        }
    }
}