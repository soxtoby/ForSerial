namespace json.Objects
{
    public interface ObjectParsingOptions
    {
        bool SerializeAllTypes { get; }
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
    }

    public class CustomObjectParsingOptions : ObjectParsingOptions
    {
        public CustomObjectParsingOptions()
        {
            SerializeAllTypes = DefaultObjectParsingOptions.Instance.SerializeAllTypes;
        }

        public bool SerializeAllTypes { get; set; }
    }
}
