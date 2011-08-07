namespace json.Objects
{
    public interface ParserValueFactory : ParseValueFactory
    {
        bool SerializeAllTypes { get; }
        ParseValue Parse(object input);
        void ParseProperty(ParseObject owner, string propertyName, TypeDefinition propertyTypeDef, object propertyValue);
        void ParseArrayItem(ParseArray array, object item);

        /// <summary>
        /// Creates a ParseObject, sets its type to the type of the input, and adds to the collection of known references.
        /// </summary>
        ParseObject CreateObject(object input);
    }
}