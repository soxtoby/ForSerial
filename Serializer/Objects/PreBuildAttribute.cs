using System;
using System.Reflection;

namespace json.Objects
{
    public abstract class PreBuildAttribute : Attribute
    {
        private readonly Type readerType;
        private readonly Type preBuildContextType;

        protected PreBuildAttribute(Type readerType, Type preBuildContextType)
        {
            this.readerType = readerType;
            this.preBuildContextType = preBuildContextType;
        }

        public bool ReaderMatches(object reader)
        {
            return reader.GetType() == readerType;
        }

        public void AssertValidMethod(MethodInfo method)
        {
            if (method.ReturnType != preBuildContextType
                || method.GetParameterTypes().SingleOrDefaultIfMore() != preBuildContextType)
            {
                throw new InvalidMethodSignature(GetType(), method, preBuildContextType);
            }
        }

        public abstract NewWriter GetWriter();

        public abstract object GetContextValue(NewWriter parsedContext);

        public abstract void ReadPreBuildResult(object preBuildResult, NewWriter writer);

        private class InvalidMethodSignature : Exception
        {
            public InvalidMethodSignature(Type attributeType, MethodInfo method, Type expectedType)
                : base("{0} applied to invalid method {1}.{2}. Single parameter and return value must both be of type {3}"
                    .FormatWith(attributeType.FullName, method.DeclaringType, method.Name, expectedType.FullName))
            { }
        }
    }
}
