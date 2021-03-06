﻿using System;
using System.Collections.Generic;
using ForSerial.Objects;

namespace ForSerial.JsonObjects
{
    public class JsonObjectWriter : Writer
    {
        private readonly Stack<JsonObject> currentObject = new Stack<JsonObject>();
        private readonly List<JsonObject> objectReferences = new List<JsonObject>();
        private string currentProperty;

        public JsonObject Result { get; private set; }

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

        public bool CanWrite(object value)
        {
            return value.IsJsonPrimitiveType();
        }

        public void Write(object value)
        {
            Write(new JsonValue(value));
        }

        public void BeginStructure(Type readerType)
        {
            JsonMap map = new JsonMap();
            Write(map);
            objectReferences.Add(map);
            currentObject.Push(map);
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
            BeginStructure(readerType);
            AddProperty("_type");
            Write(typeIdentifier);
        }

        public void EndStructure()
        {
            currentObject.Pop();
        }

        public void AddProperty(string name)
        {
            currentProperty = name;
        }

        public void BeginSequence()
        {
            JsonArray array = new JsonArray();
            Write(array);
            currentObject.Push(array);
        }

        public void EndSequence()
        {
            currentObject.Pop();
        }

        public void WriteReference(int referenceIndex)
        {
            Write(objectReferences[referenceIndex]);
        }

        private void Write(JsonObject jsonValue)
        {
            if (currentObject.Count == 0)
            {
                Result = jsonValue;
            }
            else
            {
                JsonMap map = currentObject.Peek() as JsonMap;
                if (map != null)
                    map[currentProperty] = jsonValue;
                else
                    ((JsonArray)currentObject.Peek()).Add(jsonValue);
            }
        }
    }
}