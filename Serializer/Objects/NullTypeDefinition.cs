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

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            return valueFactory.CreateValue(null);
        }
    }
}