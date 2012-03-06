namespace json.Objects
{
    public class NullTypeDefinition : TypeDefinition
    {
        private NullTypeDefinition() : base(null) { }

        public static readonly NullTypeDefinition Instance = new NullTypeDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            writer.Write(null);
        }
    }
}