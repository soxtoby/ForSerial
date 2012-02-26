﻿using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class ObjectWriter<T> : Writer
    {
        private readonly Stack<ObjectOutput> outputs = new Stack<ObjectOutput>();
        private readonly Stack<string> properties = new Stack<string>();
        private object result;

        public T Result { get { return (T)result; } }

        public bool CanWrite(object value)
        {
            bool canWrite = false;
            OnCurrent(() => canWrite = CurrentTypeHandler.GetTypeDefinition(typeof(T)).CanCreateValue(value),
                structure => canWrite = structure.CanCreateValue(properties.Peek(), value),
                sequence => canWrite = sequence.CanCreateValue(value));
            return canWrite;
        }

        public void Write(object value)
        {
            ObjectValue newValue = null;
            OnCurrent(() => newValue = CurrentTypeHandler.GetTypeDefinition(typeof(T)).CreateValue(value),
                structure => newValue = structure.CreateValue(properties.Peek(), value),
                sequence => newValue = sequence.CreateValue(value));
            AddToCurrent(newValue);
        }

        public void BeginStructure()
        {
            ObjectStructure newStructure = null;
            OnCurrent(() => newStructure = CurrentTypeHandler.GetTypeDefinition(typeof(T)).CreateStructure(),
                structure => newStructure = structure.CreateStructure(properties.Peek()),
                sequence => newStructure = ((ObjectSequence)outputs.Peek()).CreateStructure());
            outputs.Push(newStructure);
        }

        public void EndStructure()
        {
            ObjectStructure newStructure = (ObjectStructure)outputs.Pop(); // TODO throw exception if not structure
            AddToCurrent(newStructure);
        }

        public void AddProperty(string name)
        {
            properties.Push(name);
        }

        public void BeginSequence()
        {
            ObjectSequence newSequence = null;
            OnCurrent(() => newSequence = CurrentTypeHandler.GetTypeDefinition(typeof(T)).CreateSequence(),
                structure => newSequence = structure.CreateSequence(properties.Peek()),
                sequence => newSequence = ((ObjectSequence)outputs.Peek()).CreateSequence());
            outputs.Push(newSequence);
        }

        public void EndSequence()
        {
            ObjectSequence newSequence = (ObjectSequence)outputs.Pop(); // TODO throw exception if not sequence
            AddToCurrent(newSequence);
        }

        private void AddToCurrent(ObjectOutput value)
        {
            OnCurrent(() => result = value.GetTypedValue(),
                structure => structure.Add(properties.Pop(), value),
                sequence => sequence.Add(value));
        }

        private void OnCurrent(Action baseAction, Action<ObjectStructure> structureAction, Action<ObjectSequence> sequenceAction) // TODO replace with state pattern
        {
            if (outputs.None())
            {
                baseAction();
            }
            else
            {
                ObjectStructure structure = outputs.Peek() as ObjectStructure;
                if (structure != null)
                    structureAction(structure);
                else
                    sequenceAction((ObjectSequence)outputs.Peek());
            }
        }
    }
}