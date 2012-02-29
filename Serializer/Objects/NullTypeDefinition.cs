namespace json.Objects
{
    public class NullTypeDefinition : TypeDefinition
    {
        private NullTypeDefinition() : base(null) { }

        public static readonly NullTypeDefinition Instance = new NullTypeDefinition();

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            writer.Write(null);
        }
    }
}