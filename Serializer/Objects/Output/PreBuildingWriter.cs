using System;

namespace json.Objects
{
    internal class PreBuildingWriter<T> : Writer
    {
        private readonly ObjectWriter<T> primaryWriter;
        private readonly PreBuildInfo preBuildInfo;
        private readonly Writer preBuildWriter;
        private int preBuildDepth;

        public PreBuildingWriter(ObjectWriter<T> primaryWriter, PreBuildInfo preBuildInfo)
        {
            this.primaryWriter = primaryWriter;
            this.preBuildInfo = preBuildInfo;
            preBuildWriter = preBuildInfo.GetWriter();
        }

        public bool CanWrite(object value)
        {
            return preBuildWriter.CanWrite(value);
        }

        public void Write(object value)
        {
            preBuildWriter.Write(value);
        }

        public void Write(bool value) { Write((object)value); }
        public void Write(char value) { Write((object)value); }
        public void Write(decimal value) { Write((object)value); }
        public void Write(double value) { Write((object)value); }
        public void Write(float value) { Write((object)value); }
        public void Write(int value) { Write((object)value); }
        public void Write(long value) { Write((object)value); }
        public void Write(string value) { Write((object)value); }
        public void Write(uint value) { Write((object)value); }
        public void Write(ulong value) { Write((object)value); }
        public void WriteNull() { Write((object)null); }

        public void BeginStructure(Type readerType)
        {
            preBuildDepth++;
            preBuildWriter.BeginStructure(readerType);
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
            preBuildDepth++;
            preBuildWriter.BeginStructure(readerType);
        }

        public void EndStructure()
        {
            preBuildWriter.EndStructure();
            preBuildDepth--;

            if (preBuildDepth == 0)
            {
                primaryWriter.SetSubWriter(new BaseObjectWriter<T>(primaryWriter));
                preBuildInfo.PreBuild(preBuildWriter, primaryWriter);
                primaryWriter.SetSubWriter(new ObjectWriterWithPreBuilding<T>(primaryWriter));
            }
        }

        public void AddProperty(string name)
        {
            preBuildWriter.AddProperty(name);
        }

        public void BeginSequence()
        {
            preBuildWriter.BeginSequence();
        }

        public void EndSequence()
        {
            preBuildWriter.EndSequence();
        }

        public void WriteReference(int referenceIndex)
        {
            preBuildWriter.WriteReference(referenceIndex - primaryWriter.StructureReferences.Count);
        }
    }
}