namespace ForSerial.Objects.TypeDefinitions
{
    internal class NullTypeDefinition : TypeDefinition
    {
        private NullTypeDefinition() : base(null) { }

        public static readonly NullTypeDefinition Instance = new NullTypeDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            writer.WriteNull();
        }
    }
}
