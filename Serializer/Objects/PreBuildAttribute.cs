using System;
using System.Reflection;

namespace json.Objects
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class PreBuildAttribute : Attribute
    {
        private readonly Type requiredReaderType;
        private readonly Type preBuildContextType;

        protected PreBuildAttribute(Type requiredReaderType, Type preBuildContextType)
        {
            this.requiredReaderType = requiredReaderType;
            this.preBuildContextType = preBuildContextType;
        }

        public bool ReaderMatches(Type readerType)
        {
            return readerType == requiredReaderType;
        }

        public void AssertValidMethod(MethodInfo method)
        {
            if (method.ReturnType != preBuildContextType
                || method.GetParameterTypes().SingleOrDefaultIfMore() != preBuildContextType)
            {
                throw new InvalidMethodSignature(GetType(), method, preBuildContextType);
            }
        }

        public abstract Writer GetWriter();

        public abstract object GetContextValue(Writer writer);

        public abstract void ReadPreBuildResult(object preBuildResult, Writer writer);

        private class InvalidMethodSignature : Exception
        {
            public InvalidMethodSignature(Type attributeType, MethodInfo method, Type expectedType)
                : base("{0} applied to invalid method {1}.{2}. Single parameter and return value must both be of type {3}"
                    .FormatWith(attributeType.FullName, method.DeclaringType, method.Name, expectedType.FullName))
            { }
        }
    }
}
