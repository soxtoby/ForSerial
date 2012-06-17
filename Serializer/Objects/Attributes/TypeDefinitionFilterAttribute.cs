using System;
using System.Collections.Generic;

namespace ForSerial.Objects
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class TypeDefinitionFilterAttribute : Attribute
    {
        public abstract IEnumerable<FactoryMethod> Filter(IEnumerable<FactoryMethod> factoryMethods);
    }
}