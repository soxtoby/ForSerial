using System;
using System.Linq;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class ArraySequence : BaseObjectSequence
    {
        private readonly ArrayDefinition arrayDef;

        public ArraySequence(ArrayDefinition arrayDef)
            : base(arrayDef)
        {
            if (arrayDef == null) throw new ArgumentNullException("arrayDef");

            this.arrayDef = arrayDef;
        }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            return arrayDef.BuildArray(Items.Select(i => i.GetTypedValue()));
        }
    }
}