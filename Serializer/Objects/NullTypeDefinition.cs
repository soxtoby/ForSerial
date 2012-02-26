namespace json.Objects
{
    public class NullTypeDefinition : TypeDefinition
    {
        private NullTypeDefinition() : base(null) { }

        private static NullTypeDefinition instance;
        public static NullTypeDefinition Instance
        {
            get { return instance ?? (instance = new NullTypeDefinition()); }
        }

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            writer.Write(null);
        }
    }
}