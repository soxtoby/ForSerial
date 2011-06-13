using System;
using System.Reflection;

namespace json
{
    public abstract class PreBuildAttribute : Attribute
    {
        private Type parserType;
        private Type preBuildContextType;

        protected PreBuildAttribute(Type parserType, Type preBuildContextType)
        {
            this.parserType = parserType;
            this.preBuildContextType = preBuildContextType;
        }

        public bool ParserMatches(Parser parser)
        {
            return parser.GetType() == parserType;
        }

        public void AssertValidMethod(MethodInfo method)
        {
            if (method.ReturnType != preBuildContextType
                || method.GetParameterTypes().SingleOrDefaultIfMore() != preBuildContextType)
            {
                throw new InvalidMethodSignature(GetType(), method, preBuildContextType);
            }
        }

        public abstract ParseValueFactory GetBuilder();

        public abstract object GetContextValue(ParseObject parsedContext);

        public abstract void ParsePreBuildResult(object preBuildResult, ParseValueFactory valueFactory);

        private class InvalidMethodSignature : Exception
        {
            public InvalidMethodSignature(Type attributeType, MethodInfo method, Type expectedType)
                : base("{0} applied to invalid method {1}.{2}. Single parameter and return value must both be of type {3}"
                    .FormatWith(attributeType.FullName, method.DeclaringType, method.Name, expectedType.FullName))
            { }
        }
    }
}
