﻿using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectWriter<T> : Writer
    {
        internal readonly Stack<ObjectContainer> Outputs = new Stack<ObjectContainer>();
        internal readonly List<ObjectContainer> StructureReferences = new List<ObjectContainer>();
        private Writer subWriter;

        public ObjectWriter()
        {
            Outputs.Push(new BaseOutput<T>());
            subWriter = new ObjectWriterWithPreBuilding<T>(this);
        }

        public T Result { get { return (T)Outputs.Peek().GetTypedValue(); } }

        public bool CanWrite(object value)
        {
            return subWriter.CanWrite(value);
        }

        public void Write(object value)
        {
            subWriter.Write(value);
        }

        public void BeginStructure(Type readerType)
        {
            subWriter.BeginStructure(readerType);
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
            subWriter.BeginStructure(typeIdentifier, readerType);
        }

        public void EndStructure()
        {
            subWriter.EndStructure();
        }

        public void AddProperty(string name)
        {
            subWriter.AddProperty(name);
        }

        public void BeginSequence()
        {
            subWriter.BeginSequence();
        }

        public void EndSequence()
        {
            subWriter.EndSequence();
        }

        public void WriteReference(int referenceIndex)
        {
            subWriter.WriteReference(referenceIndex);
        }

        internal void SetSubWriter(Writer writer)
        {
            subWriter = writer;
        }
    }
}