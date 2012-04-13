using System;

namespace ForSerial.Objects
{
    public class ParameterDefinition
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public ParameterDefinition(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}