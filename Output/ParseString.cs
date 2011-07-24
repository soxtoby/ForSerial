namespace json
{
    public abstract class ParseString : ParseValue
    {
        protected readonly string value;

        protected ParseString(string value)
        {
            this.value = value;
        }

        public abstract void AddToObject(ParseObject obj, string name);
        public abstract void AddToArray(ParseArray array);
        public abstract ParseObject AsObject();
    }
}