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

        public void PreBuild(object target, Reader reader, Writer objectPopulator)
        {
            Writer writerForContext = attribute.GetWriter();
            OutputStructure contextOutput = reader.ReadSubStructure(writerForContext);
            object context = attribute.GetContextValue(contextOutput);

            object preBuildResult = method.Invoke(target, new[] { context });

            attribute.ReadPreBuildResult(preBuildResult, objectPopulator);
        }

        public bool ReaderMatches(Reader reader)
        {
            return attribute.ReaderMatches(reader);
        }
    }
}