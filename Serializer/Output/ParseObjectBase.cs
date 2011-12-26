namespace json
{
    public abstract class ParseObjectBase : ParseObject
    {
        public abstract void AddToObject(ParseObject obj, string name);

        public abstract void AddToArray(ParseArray array);

        public ParseObject AsObject()
        {
            return this;
        }

        public virtual bool SetType(string typeIdentifier, Parser parser) { return false; }

        public virtual ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value)
        {
            return valueFactory.CreateValue(value);
        }

        public virtual ParseObject CreateObject(string name, ParseValueFactory valueFactory)
        {
            return valueFactory.CreateObject();
        }

        public virtual ParseArray CreateArray(string name, ParseValueFactory valueFactory)
        {
            return valueFactory.CreateArray();
        }
    }
}