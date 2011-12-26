namespace json
{
    public interface ParseArray : ParseValue
    {
        ParseValue CreateValue(ParseValueFactory valueFactory, object value);
        ParseObject CreateObject(ParseValueFactory valueFactory);
        ParseArray CreateArray(ParseValueFactory valueFactory);
    }
}