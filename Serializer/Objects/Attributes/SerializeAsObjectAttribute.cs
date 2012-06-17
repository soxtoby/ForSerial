using System.Collections.Generic;
using System.Linq;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class SerializeAsObjectAttribute : TypeDefinitionFilterAttribute
    {
        public override IEnumerable<FactoryMethod> Filter(IEnumerable<FactoryMethod> factoryMethods)
        {
            return factoryMethods
                .Where(m => !m.Method.ReturnType.CanBeCastTo(typeof(SequenceDefinition)));
        }
    }
}