using System;

namespace json.JsonObjects
{
    public class JsonObjectReader : JObjectVisitor
    {
        private readonly Writer writer;
        private const string TypeKey = "_type";
        private JsonMap currentObject;
        //private readonly Dictionary<JsonMap, OutputStructure> objectReferences = new Dictionary<JsonMap, OutputStructure>();

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

        // TODO reimplement ReadSubStructure
        //public override Output ReadSubStructure(Writer subWriter)
        //{
        //    return Read(currentObject, subWriter);
        //}

        private void ReadValue(JsonObject input)
        {
            if (input == null)
            {
                writer.Write(null);
                return;
            }

            input.Accept(this);
        }

        public void Visit(JsonMap map)
        {
            ReadNewObject(map);

            //OutputStructure existingReference = objectReferences.Get(obj);
            //existingReference == null
            //    ? ReadNewObject(obj)
            //    : ReferenceObject(existingReference);
        }

        private void ReadNewObject(JsonMap map)
        {
            currentObject = map;

            writer.BeginStructure();

            foreach (var property in map)
            {
                // TODO reimplement SetType
                //if (name == TypeKey)
                //    writer.SetType((string)map[TypeKey], this);
                //else

                writer.AddProperty(property.Key);
                ReadValue(property.Value);
            }

            writer.EndStructure();
        }

        // TODO reimplement object references
        //private OutputStructure ReferenceObject(OutputStructure existingReference)
        //{
        //    return writer.CreateReference(existingReference);
        //}

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
