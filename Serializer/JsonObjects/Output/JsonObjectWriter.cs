using System.Collections.Generic;
using json.Objects;

namespace json.JsonObjects
{
    public class JsonObjectWriter : Writer
    {
        private readonly Stack<JsonObject> currentObject = new Stack<JsonObject>();
        private readonly List<JsonObject> objectReferences = new List<JsonObject>();
        private string currentProperty;

        public JsonObject Result { get; private set; }

        public bool CanWrite(object value)
        {
            return value.IsJsonPrimitiveType();
        }

        public void Write(object value)
        {
            Write(new JsonValue(value));
        }

        public void BeginStructure()
        {
            JsonMap map = new JsonMap();
            Write(map);
            objectReferences.Add(map);
            currentObject.Push(map);
        }

        public void SetType(string typeIdentifier)
        {
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