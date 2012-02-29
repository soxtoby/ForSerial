using System.Collections;
using System.Collections.Generic;

namespace json.JsonObjects
{
    public class JsonArray : JsonObject, IEnumerable<JsonObject>
    {
        private readonly List<JsonObject> innerArray = new List<JsonObject>();

        public void Add(bool value) { Add(new JsonValue(value)); }
        public void Add(int value) { Add(new JsonValue(value)); }
        public void Add(double value) { Add(new JsonValue(value)); }
        public void Add(float value) { Add(new JsonValue(value)); }
        public void Add(string value) { Add(new JsonValue(value)); }
        public void Add(char value) { Add(new JsonValue(value + string.Empty)); }

        public void Add(JsonObject value)
        {
            innerArray.Add(value);
        }

        public JsonObject this[int index]
        {
            get
            {
                if (index < 0 || index >= innerArray.Count)
                    return null;
                return innerArray[index];
            }
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            return innerArray.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Accept(JObjectVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}