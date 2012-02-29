using System;

namespace json.Objects
{
    internal class ObjectWriterWithPreBuilding<T> : BaseObjectWriter<T>
    {
        public ObjectWriterWithPreBuilding(ObjectWriter<T> primaryWriter)
            : base(primaryWriter)
        {
        }

        protected override void BeginStructure(Type readerType, Func<ObjectContainer, ObjectContainer> createStructure, Action<Writer> initialPreBuildWrite)
        {
            ObjectContainer newStructure = createStructure(Outputs.Peek());

            PreBuildInfo preBuildInfo = newStructure.GetPreBuildInfo(readerType);
            if (preBuildInfo != null)
            {
                PrimaryWriter.SetSubWriter(new PreBuildingWriter<T>(PrimaryWriter, preBuildInfo));
                initialPreBuildWrite(PrimaryWriter);
            }
            else
            {
                StructureReferences.Add(newStructure);
                Outputs.Push(newStructure);
            }
        }
    }
}