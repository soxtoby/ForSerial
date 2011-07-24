namespace json
{
    public abstract class ParseArrayBase : ParseArray
    {
        public abstract void AddToObject(ParseObject obj, string name);
        public abstract void AddToArray(ParseArray array);
        public abstract ParseObject AsObject();

        public virtual ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return valueFactory.CreateValue(value);
        }

        public virtual ParseObject CreateObject(ParseValueFactory valueFactory)
        {
            return valueFactory.CreateObject();
        }

        public virtual ParseArray CreateArray(ParseValueFactory valueFactory)
        {
            return valueFactory.CreateArray();
        }
    }
}