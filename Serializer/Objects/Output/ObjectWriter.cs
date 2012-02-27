using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectWriter<T> : Writer
    {
        private readonly Stack<ObjectContainer> outputs = new Stack<ObjectContainer>();
        private readonly List<ObjectContainer> structureReferences = new List<ObjectContainer>();

        public ObjectWriter()
        {
            outputs.Push(new BaseOutput<T>());
        }

        public T Result { get { return (T)outputs.Peek().GetTypedValue(); } }

        public bool CanWrite(object value)
        {
            return outputs.Peek().CanCreateValue(value);
        }

        public void Write(object value)
        {
            outputs.Peek().WriteValue(value);
        }

        public void BeginStructure()
        {
            ObjectContainer newStructure = outputs.Peek().CreateStructure();
            structureReferences.Add(newStructure);
            outputs.Push(newStructure);
        }

        public void SetType(string typeIdentifier)
        {
            outputs.Peek().SetType(typeIdentifier);
        }

        public void EndStructure()
        {
            ObjectOutput newStructure = outputs.Pop(); // TODO throw exception if not structure
            outputs.Peek().Add(newStructure);
        }

        public void AddProperty(string name)
        {
            outputs.Peek().SetCurrentProperty(name);
        }

        public void BeginSequence()
        {
            outputs.Push(outputs.Peek().CreateSequence());
        }

        public void EndSequence()
        {
            ObjectOutput newSequence = outputs.Pop(); // TODO throw exception if not sequence
            outputs.Peek().Add(newSequence);
        }

        public void WriteReference(int referenceIndex)
        {
            outputs.Peek().Add(new StructurerReference(structureReferences[referenceIndex]));
        }

        public void AddStructureReference(ObjectContainer structureOutput)
        {
            structureReferences.Add(structureOutput);
        }
    }
}