namespace json.Objects
{
    public interface ParserValueFactory : ParseValueFactory
    {
        bool SerializeAllTypes { get; }
        ParseValue Parse(object input);
        ParseValue ParseProperty(ParseObject owner, string propertyName, object propertyValue);
    }
}