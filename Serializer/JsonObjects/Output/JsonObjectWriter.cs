using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace json.JsonObjects
{
    public class JsonObjectWriter : NewWriter
    {
        private readonly Stack<JObject> currentObject = new Stack<JObject>();
        private string currentProperty;

        public JObject Result { get; private set; }

        public void WriteValue(object value)
        {
            Write(new JsonValue(value));
        }

        public void BeginStructure()
        {
            JsonMap map = new JsonMap();
            Write(map);
            currentObject.Push(map);
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

        private void Write(JObject jsonValue)
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

    public interface JObject
    {
        void Accept(JObjectVisitor visitor);
    }

    public interface JObjectVisitor
    {
        void Visit(JsonMap map);
        void Visit(JsonArray array);
        void Visit(JsonValue value);
    }

    public class JsonMap : JObject, IEnumerable<KeyValuePair<string, JObject>>
    {
        private readonly Dictionary<string, JObject> innerMap = new Dictionary<string, JObject>();

        public JObject this[string key]
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

        public void Add(string key, JObject value)
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

        public void Accept(JObjectVisitor visitor)
        {
            visitor.Visit(this);
        }

        public IEnumerator<KeyValuePair<string, JObject>> GetEnumerator()
        {
            return innerMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class JsonArray : JObject, IEnumerable<JObject>
    {
        private readonly List<JObject> innerArray = new List<JObject>();

        public void Add(bool value) { Add(new JsonValue(value)); }
        public void Add(int value) { Add(new JsonValue(value)); }
        public void Add(double value) { Add(new JsonValue(value)); }
        public void Add(float value) { Add(new JsonValue(value)); }
        public void Add(string value) { Add(new JsonValue(value)); }
        public void Add(char value) { Add(new JsonValue(value + string.Empty)); }

        public void Add(JObject value)
        {
            innerArray.Add(value);
        }

        public IEnumerator<JObject> GetEnumerator()
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

    public class JsonValue : JObject
    {
        public object Value { get; private set; }

        public JsonValue(object value)
        {
            Value = value;
        }

        public void Accept(JObjectVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public static class JObjectExtensions
    {
        public static JObject Get(this JObject obj, params string[] properties)
        {
            foreach (string property in properties)
            {
                JsonMap map = obj as JsonMap;

                if (map == null)
                    return null;

                obj = map[property];
            }

            return obj;
        }

        public static object Value(this JsonMap map, string key)
        {
            return map[key].Value();
        }

        public static object Value(this JObject obj)
        {
            JsonValue value = obj as JsonValue;
            return value == null
                ? null
                : value.Value;
        }

        public static IEnumerable<object> Values(this IEnumerable<JObject> array)
        {
            return array.OfType<JsonValue>().Select(Value);
        }
    }
}