namespace json
{
    public interface ParseValue
    {
        void AddToObject(ParseObject obj, string name);
        void AddToArray(ParseArray array);
        /// <summary>
        /// Converts the current value into a <see cref="ParseObject"/>, so parser output is consistent.
        /// </summary>
        ParseObject AsObject();
    }

    public interface ParseObject : ParseValue
    {
        string TypeIdentifier { get; set; }
        void AddNumber(string name, double value);
        void AddString(string name, string value);
        void AddObject(string name, ParseObject value);
        void AddArray(string name, ParseArray value);
    }

    public interface ParseArray : ParseValue
    {
        void AddNumber(double value);
        void AddString(string value);
        void AddObject(ParseObject value);
        void AddArray(ParseArray value);
    }

    public interface ParseNumber : ParseValue { }

    public interface ParseString : ParseValue { }

    public interface ParseValueFactory
    {
        ParseObject CreateObject();
        ParseArray CreateArray();
        ParseNumber CreateNumber(double value);
        ParseString CreateString(string value);
    }
}

