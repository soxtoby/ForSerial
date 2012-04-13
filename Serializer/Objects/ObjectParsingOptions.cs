namespace ForSerial.Objects
{
    public class ObjectParsingOptions
    {
        public TypeInformationLevel SerializeTypeInformation { get; set; }
        public PropertyFilter PropertyFilter { get; set; }
        public string Scenario { get; set; }
    }

    public enum TypeInformationLevel
    {
        Minimal,
        All,
        None
    }

    public enum PropertyFilter
    {
        PublicGet,
        PublicGetSet
    }
}
