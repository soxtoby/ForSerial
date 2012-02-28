using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectWriter<T> : Writer
    {
        private readonly Stack<ObjectContainer> outputs = new Stack<ObjectContainer>();
        private readonly List<ObjectContainer> structureReferences = new List<ObjectContainer>();
        private PreBuildInfo preBuildInfo;
        private Writer preBuildWriter;
        private int preBuildDepth;

        public ObjectWriter()
        {
            outputs.Push(new BaseOutput<T>());
        }

        public T Result { get { return (T)outputs.Peek().GetTypedValue(); } }

        public bool CanWrite(object value)
        {
            return preBuildWriter != null
                ? preBuildWriter.CanWrite(value)
                : outputs.Peek().CanCreateValue(value);
        }

        public void Write(object value)
        {
            if (preBuildWriter != null)
                preBuildWriter.Write(value);
            else
                outputs.Peek().WriteValue(value);
        }

        public void BeginStructure(Type readerType)
        {
            BeginStructure(readerType, container => container.CreateStructure(), () => BeginStructure(readerType), () => preBuildWriter.BeginStructure(readerType));
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
            BeginStructure(readerType, container => container.CreateStructure(typeIdentifier), () => BeginStructure(typeIdentifier, readerType), () => preBuildWriter.BeginStructure(readerType));
        }

        private void BeginStructure(Type readerType, Func<ObjectContainer, ObjectContainer> createStructure, Action initialPreBuildWrite, Action delegateToPreBuildWriter)
        {
            if (preBuildWriter != null)
            {
                preBuildDepth++;
                delegateToPreBuildWriter();
            }
            else
            {
                ObjectContainer newStructure = createStructure(outputs.Peek());

                CheckForPreBuild(readerType, initialPreBuildWrite, newStructure);

                if (preBuildWriter == null)
                {
                    structureReferences.Add(newStructure);
                    outputs.Push(newStructure);
                }
            }
        }

        private void CheckForPreBuild(Type readerType, Action initialPreBuildWrite, ObjectContainer newStructure)
        {
            if (preBuildInfo == null) // So PreBuild doesn't trigger another PreBuild
            {
                preBuildInfo = newStructure.GetPreBuildInfo(readerType);
                if (preBuildInfo != null)
                {
                    preBuildWriter = preBuildInfo.GetWriter();
                    initialPreBuildWrite();
                }
            }
        }

        public void EndStructure()
        {
            if (preBuildWriter != null)
            {
                preBuildWriter.EndStructure();
                preBuildDepth--;
                if (preBuildDepth == 0)
                {
                    Writer writer = preBuildWriter;
                    preBuildWriter = null;
                    preBuildInfo.PreBuild(writer, this);
                }
            }
            else
            {
                ObjectOutput newStructure = outputs.Pop(); // TODO throw exception if not structure
                outputs.Peek().Add(newStructure);
            }
        }

        public void AddProperty(string name)
        {
            if (preBuildWriter != null)
                preBuildWriter.AddProperty(name);
            else
                outputs.Peek().SetCurrentProperty(name);
        }

        public void BeginSequence()
        {
            if (preBuildWriter != null)
                preBuildWriter.BeginSequence();
            else
                outputs.Push(outputs.Peek().CreateSequence());
        }

        public void EndSequence()
        {
            if (preBuildWriter != null)
            {
                preBuildWriter.EndSequence();
            }
            else
            {
                ObjectOutput newSequence = outputs.Pop(); // TODO throw exception if not sequence
                outputs.Peek().Add(newSequence);
            }
        }

        public void WriteReference(int referenceIndex)
        {
            if (preBuildWriter != null)
                preBuildWriter.WriteReference(referenceIndex - structureReferences.Count);
            else
                outputs.Peek().Add(new StructurerReference(structureReferences[referenceIndex]));
        }
    }
}