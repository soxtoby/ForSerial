namespace json
{
    public abstract class ParseBoolean : ParseValue
    {
        protected readonly bool value;

        protected ParseBoolean(bool value)
        {
            this.value = value;
        }

        public abstract void AddToObject(ParseObject obj, string name);
        public abstract void AddToArray(ParseArray array);
        public abstract ParseObject AsObject();
    }
}