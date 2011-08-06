namespace json.Objects
{
    public interface ObjectParsingOptions
    {
        bool SerializeAllTypes { get; }
        TypeHandler TypeHandler { get; }
    }

    public class DefaultObjectParsingOptions : ObjectParsingOptions
    {
        private DefaultObjectParsingOptions() { }

        private static DefaultObjectParsingOptions instance;
        public static DefaultObjectParsingOptions Instance
        {
            get { return instance ?? (instance = new DefaultObjectParsingOptions()); }
        }

        public bool SerializeAllTypes { get { return false; } }
        public TypeHandler TypeHandler { get { return DefaultTypeHandler.Instance; } }
    }

    public class CustomObjectParsingOptions : ObjectParsingOptions
    {
        public CustomObjectParsingOptions()
        {
            SerializeAllTypes = DefaultObjectParsingOptions.Instance.SerializeAllTypes;
            TypeHandler = DefaultObjectParsingOptions.Instance.TypeHandler;
        }

        public bool SerializeAllTypes { get; set; }
        public TypeHandler TypeHandler { get; set; }
    }
}
