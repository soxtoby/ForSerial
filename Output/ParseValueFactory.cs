namespace json
{
    public interface ParseValueFactory
    {
        ParseObject CreateObject();
        ParseArray CreateArray();
        ParseNumber CreateNumber(double value);
        ParseString CreateString(string value);
        ParseBoolean CreateBoolean(bool value);
        ParseNull CreateNull();
        ParseObject CreateReference(ParseObject parseObject);
    }
}