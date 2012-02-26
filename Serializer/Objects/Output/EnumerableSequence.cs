using System.Linq;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public class EnumerableSequence : BaseObjectSequence
    {
        public EnumerableSequence(EnumerableDefinition collectionDef) : base(collectionDef) { }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            return Items.Select(i => i.GetTypedValue()).ToList();
        }
    }
}