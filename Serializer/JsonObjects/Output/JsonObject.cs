using System.Collections.Generic;
using System.Linq;

namespace json.JsonObjects
{
    public interface JsonObject
    {
        void Accept(JObjectVisitor visitor);
    }

    public static class JsonObjectExtensions
    {
        public static JsonObject Get(this JsonObject obj, params string[] properties)
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
            return Value(map[key]);
        }

        public static object Value(this JsonObject obj)
        {
            JsonValue value = obj as JsonValue;
            return value == null
                ? null
                : value.Value;
        }

        public static IEnumerable<object> Values(this IEnumerable<JsonObject> array)
        {
            return array.OfType<JsonValue>().Select(Value);
        }
    }
}