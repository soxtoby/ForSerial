using System;

namespace json.Objects
{
    internal class PreBuildInfo
    {
        private readonly PreBuildAttribute attribute;
        private readonly FuncMethod preBuild;

        public PreBuildInfo(PreBuildAttribute attribute, FuncMethod preBuild)
        {
            if (preBuild == null) throw new ArgumentNullException("preBuild");
            this.attribute = attribute;
            this.preBuild = preBuild;
        }

        public void PreBuild(object target, object reader, Writer objectPopulator)
        {
            Writer writerForContext = attribute.GetWriter();
            // reader.ReadSubStructure(writerForContext);
            object context = attribute.GetContextValue(writerForContext);

            object preBuildResult = preBuild(target, new[] { context });

            attribute.ReadPreBuildResult(preBuildResult, objectPopulator);
        }

        public bool ReaderMatches(object reader)
        {
            return attribute.ReaderMatches(reader);
        }
    }
}