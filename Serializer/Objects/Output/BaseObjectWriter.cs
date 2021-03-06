using System;
using System.Collections.Generic;

namespace ForSerial.Objects
{
    internal class BaseObjectWriter<T> : Writer
    {
        protected readonly ObjectWriter<T> PrimaryWriter;
        protected Stack<ObjectContainer> Outputs { get { return PrimaryWriter.Outputs; } }
        protected List<ObjectContainer> StructureReferences { get { return PrimaryWriter.StructureReferences; } }

        protected internal BaseObjectWriter(ObjectWriter<T> primaryWriter)
        {
            PrimaryWriter = primaryWriter;
        }

        public bool CanWrite(object value)
        {
            return Outputs.Peek().CanCreateValue(value);
        }

        public void Write(object value)
        {
            Outputs.Peek().WriteValue(value);
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
            BeginStructure(readerType, container => container.CreateStructure(), writer => writer.BeginStructure(readerType));
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
            BeginStructure(readerType, container => container.CreateStructure(typeIdentifier), writer => writer.BeginStructure(typeIdentifier, readerType));
        }

        protected virtual void BeginStructure(Type readerType, Func<ObjectContainer, ObjectContainer> createStructure, Action<Writer> initialPreBuildWrite)
        {
            ObjectContainer newStructure = createStructure(Outputs.Peek());
            StructureReferences.Add(newStructure);
            Outputs.Push(newStructure);
        }

        public void EndStructure()
        {
            ObjectOutput newStructure = Outputs.Pop(); // TODO throw exception if not structure
            Outputs.Peek().Add(newStructure);
        }

        public void AddProperty(string name)
        {
            Outputs.Peek().SetCurrentProperty(name);
        }

        public void BeginSequence()
        {
            Outputs.Push(Outputs.Peek().CreateSequence());
        }

        public void EndSequence()
        {
            ObjectOutput newSequence = Outputs.Pop(); // TODO throw exception if not sequence
            Outputs.Peek().Add(newSequence);
        }

        public void WriteReference(int referenceIndex)
        {
            Outputs.Peek().Add(new StructurerReference(StructureReferences[referenceIndex]));
        }
    }
}