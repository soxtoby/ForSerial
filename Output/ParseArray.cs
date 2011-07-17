namespace json
{
    public interface ParseArray : ParseValue
    {
        void AddNull();
        void AddBoolean(bool value);
        void AddNumber(double value);
        void AddString(string value);
        void AddObject(ParseObject value);
        void AddArray(ParseArray value);
        ParseObject CreateObject(ParseValueFactory valueFactory);
        ParseArray CreateArray(ParseValueFactory valueFactory);
    }
}