using System;
using System.Collections.Generic;

namespace json.JsonObjects
{
    public class JsonObjectReader : JObjectVisitor
    {
        private readonly Writer writer;
        private const string TypeKey = "_type";
        private readonly Dictionary<JsonMap, int> objectReferences = new Dictionary<JsonMap, int>();

        private JsonObjectReader(Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public static void Read(JsonObject obj, Writer writer)
        {
            JsonObjectReader reader = new JsonObjectReader(writer);
            reader.ReadValue(obj);
        }

        private void ReadValue(JsonObject input)
        {
            if (input == null)
                writer.WriteNull();
            else
                input.Accept(this);
        }

        public void Visit(JsonMap map)
        {
            if (objectReferences.ContainsKey(map))
                ReferenceObject(objectReferences[map]);
            else
                ReadNewObject(map);
        }

        private void ReadNewObject(JsonMap map)
        {
            objectReferences[map] = objectReferences.Count;

            if (((string)map[TypeKey].Value()).IsNotNullOrEmpty())
                writer.BeginStructure((string)map[TypeKey].Value(), GetType());
            else
                writer.BeginStructure(GetType());

            foreach (KeyValuePair<string, JsonObject> property in map)
            {
                string name = property.Key;
                JsonObject value = property.Value;

                if (name != TypeKey)
                {
                    writer.AddProperty(name);
                    ReadValue(value);
                }
            }

            writer.EndStructure();
        }

        private void ReferenceObject(int referenceIndex)
        {
            writer.WriteReference(referenceIndex);
        }

        public void Visit(JsonArray array)
        {
            writer.BeginSequence();

            foreach (JsonObject item in array)
                ReadValue(item);

            writer.EndSequence();
        }

        public void Visit(JsonValue value)
        {
            writer.Write(value.Value);
        }
    }
}
