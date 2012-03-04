namespace json.Objects
{
    public class ObjectParsingOptions
    {
        public TypeInformationLevel SerializeTypeInformation { get; set; }
    }

    public enum TypeInformationLevel
    {
        Minimal,
        All,
        None
    }
}
