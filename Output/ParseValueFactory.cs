namespace json
{
    public interface ParseValueFactory
    {
        ParseValue CreateValue(object value);
        ParseObject CreateObject();
        ParseArray CreateArray();
        ParseObject CreateReference(ParseObject parseObject);
    }
}