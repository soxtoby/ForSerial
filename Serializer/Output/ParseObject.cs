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
        ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value);
        ParseObject CreateObject(string name, ParseValueFactory valueFactory);
        ParseArray CreateArray(string name, ParseValueFactory valueFactory);
    }
}