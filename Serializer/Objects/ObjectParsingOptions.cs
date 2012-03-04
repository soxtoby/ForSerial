namespace json.Objects
{
    public class ObjectParsingOptions
    {
        public bool SerializeAllTypes { get; set; }
        public TypeInformationLevel SerializeTypeInformation { get; set; }
    }

    public enum TypeInformationLevel
    {
        Minimal,
        All,
        None
    }
}
