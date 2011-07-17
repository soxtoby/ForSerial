namespace json
{
    public abstract class ParseArrayBase : ParseArray
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddArray(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddArray(this);
        }

        public abstract ParseObject AsObject();

        public abstract void AddNull();

        public abstract void AddBoolean(bool value);

        public abstract void AddNumber(double value);

        public abstract void AddString(string value);

        public abstract void AddObject(ParseObject value);

        public abstract void AddArray(ParseArray value);

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