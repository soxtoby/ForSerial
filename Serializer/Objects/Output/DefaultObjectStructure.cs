using System.Collections.Generic;
using System.Linq;

namespace json.Objects
{
    public class DefaultObjectStructure : BaseObjectStructure
    {
        public DefaultObjectStructure(TypeDefinition typeDef) : base(typeDef) { }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            IEnumerable<ConstructorDefinition> constructors = TypeDef.Constructors;
            ConstructorDefinition matchingConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

            if (matchingConstructor == null)
                throw new NoMatchingConstructor(TypeDef.Type, Properties);

            object[] parameters = matchingConstructor.Parameters
                .Select(GetParameterPropertyValue)
                .ToArray();

            object obj = matchingConstructor.Construct(parameters);

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
            {
                PropertyDefinition propDef = TypeDef.Properties.Get(property.Key);
                if (propDef != null)
                    property.Value.AssignToProperty(obj, propDef);
            }

            return obj;
        }
    }
}