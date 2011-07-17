namespace json
{
    public abstract class ParseObjectBase : ParseObject
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddObject(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddObject(this);
        }

        public ParseObject AsObject()
        {
            return this;
        }

        public virtual bool SetType(string typeIdentifier, Parser parser) { return false; }

        public abstract void AddNull(string name);

        public abstract void AddBoolean(string name, bool value);

        public abstract void AddNumber(string name, double value);

        public abstract void AddString(string name, string value);

        public abstract void AddObject(string name, ParseObject value);

        public abstract void AddArray(string name, ParseArray value);

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