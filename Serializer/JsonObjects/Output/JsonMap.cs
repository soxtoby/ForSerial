using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForSerial.JsonObjects
{
    public class JsonMap : JsonObject, IEnumerable<KeyValuePair<string, JsonObject>>
    {
        private readonly Dictionary<string, JsonObject> innerMap = new Dictionary<string, JsonObject>();

        public JsonObject this[string key]
        {
            get { return innerMap.Get(key); }
            set { innerMap[key] = value; }
        }

        public int Count
        {
            get { return innerMap.Count; }
        }

        public void Add(string key, bool value) { Add(key, new JsonValue(value)); }
        public void Add(string key, int value) { Add(key, new JsonValue(value)); }
        public void Add(string key, double value) { Add(key, new JsonValue(value)); }
        public void Add(string key, float value) { Add(key, new JsonValue(value)); }
        public void Add(string key, string value) { Add(key, new JsonValue(value)); }
        public void Add(string key, char value) { Add(key, new JsonValue(value + string.Empty)); }

        public void Add(string key, JsonObject value)
        {
            innerMap.Add(key, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(JsonMap)) return false;

            JsonMap other = (JsonMap)obj;
            return Count == other.Count
                && innerMap.Keys.All(key => Equals(this[key], other[key]));
        }

        public override int GetHashCode()
        {
            return innerMap.GetHashCode();
        }

        void JsonObject.Accept(JsonObjectVisitor visitor)
        {
            visitor.Visit(this);
        }

        public IEnumerator<KeyValuePair<string, JsonObject>> GetEnumerator()
        {
            return innerMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}