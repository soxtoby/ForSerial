namespace json
{
    public abstract class ParseString : ParseValue
    {
        protected readonly string value;

        protected ParseString(string value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddString(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddString(value);
        }

        public abstract ParseObject AsObject();
    }
}