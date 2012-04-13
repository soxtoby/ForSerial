namespace ForSerial.Objects.TypeDefinitions
{
    internal class ObjectDefinition : TypeDefinition
    {
        private ObjectDefinition() : base(typeof(object)) { }

        public static readonly ObjectDefinition Instance = new ObjectDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (reader.ReferenceStructure(input))
                return;
            writer.Write(input);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(value);
        }
    }
}
