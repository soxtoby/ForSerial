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
}

