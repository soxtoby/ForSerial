namespace ForSerial.Objects.TypeDefinitions
{
    internal class ObjectDefinition : StructureDefinition
    {
        private ObjectDefinition() : base(typeof(object)) { }

        public static readonly ObjectDefinition Instance = new ObjectDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            if (ReferenceStructure(input, reader, optionsOverride))
                return;
            writer.Write(input);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(value);
        }
    }
}
