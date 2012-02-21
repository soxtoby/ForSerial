using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class ConstructorDefinition
    {
        public ConstructorMethod Construct { get; private set; }
        public IEnumerable<ParameterDefinition> Parameters { get; private set; }

        public ConstructorDefinition(ConstructorMethod getConstructor, IEnumerable<ParameterDefinition> parameters)
        {
            if (getConstructor == null) throw new ArgumentNullException("getConstructor");
            if (parameters == null) throw new ArgumentNullException("parameters");

            Construct = getConstructor;
            Parameters = parameters;
        }
    }
}