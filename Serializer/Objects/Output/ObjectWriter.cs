using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectWriter<T> : Writer
    {
        private readonly Stack<ObjectContainer> outputs = new Stack<ObjectContainer>();

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
            outputs.Push(outputs.Peek().CreateStructure());
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
    }
}