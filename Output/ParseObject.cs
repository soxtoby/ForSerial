namespace json
{
    public interface ParseObject : ParseValue
    {
        /// <summary>
        /// Sets the object's type.
        /// </summary>
        /// <returns>
        /// True if the object was pre-built and the parser should skip populating it.
        /// </returns>
        bool SetType(string typeIdentifier, Parser parser);
        void AddNull(string name);
        void AddBoolean(string name, bool value);
        void AddNumber(string name, double value);
        void AddString(string name, string value);
        void AddObject(string name, ParseObject value);
        void AddArray(string name, ParseArray value);
        ParseObject CreateObject(string name, ParseValueFactory valueFactory);
        ParseArray CreateArray(string name, ParseValueFactory valueFactory);
    }
}