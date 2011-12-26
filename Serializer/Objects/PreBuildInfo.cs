using System.Reflection;

namespace json.Objects
{
    internal class PreBuildInfo
    {
        private readonly PreBuildAttribute attribute;
        private readonly MethodInfo method;

        public PreBuildInfo(PreBuildAttribute attribute, MethodInfo method)
        {
            this.attribute = attribute;
            this.method = method;
        }

        public void PreBuild(object target, Parser parser, ParseValueFactory objectPopulator)
        {
            ParseValueFactory contextBuilder = attribute.GetBuilder();
            ParseObject parsedContext = parser.ParseSubObject(contextBuilder);
            object context = attribute.GetContextValue(parsedContext);

            object preBuildResult = method.Invoke(target, new[] { context });

            attribute.ParsePreBuildResult(preBuildResult, objectPopulator);
        }

        public bool ParserMatches(Parser parser)
        {
            return attribute.ParserMatches(parser);
        }
    }
}