namespace json.Objects
{
    public interface ParserValueFactory : ParseValueFactory
    {
        bool SerializeAllTypes { get; }
        void ParseProperty(object source, PropertyDefinition property, ParseObject target);
        void ParseProperty(TypeDefinition propertyTypeDef, string propertyName, object propertyValue, ParseObject target);
        void ParseArrayItem(ParseArray array, object item);

        /// <summary>
        /// Creates a ParseObject, sets its type to the type of the input, and adds to the collection of known references.
        /// </summary>
        ParseObject CreateObject(object input);
    }
}