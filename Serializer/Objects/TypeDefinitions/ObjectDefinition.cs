using System;

namespace json.Objects.TypeDefinitions
{
    internal class ObjectDefinition : TypeDefinition
    {
        private ObjectDefinition() : base(typeof(object)) { }

        internal static ObjectDefinition CreateObjectDefinition(Type type)
        {
            return type == typeof(object)
                ? new ObjectDefinition()
                : null;
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (reader.ReferenceStructure(input))
                return;
            writer.Write(input);
        }

        public override ObjectValue CreateValue(object value)
        {
            return new DefaultObjectValue(value);
        }
    }
}
