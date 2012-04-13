using System;

namespace ForSerial.Objects
{
    public class PreBuildInfo
    {
        private readonly PreBuildAttribute attribute;
        private readonly StaticFuncMethod preBuild;

        public PreBuildInfo(PreBuildAttribute attribute, StaticFuncMethod preBuild)
        {
            if (preBuild == null) throw new ArgumentNullException("preBuild");
            this.attribute = attribute;
            this.preBuild = preBuild;
        }

        public bool ReaderMatches(Type readerType)
        {
            return attribute.ReaderMatches(readerType);
        }

        public Writer GetWriter()
        {
            return attribute.GetWriter();
        }

        public void PreBuild(Writer preBuildWriter, Writer primaryWriter)
        {
            object context = attribute.GetContextValue(preBuildWriter);
            object preBuildResult = preBuild(new[] { context });
            attribute.ReadPreBuildResult(preBuildResult, primaryWriter);
        }
    }
}