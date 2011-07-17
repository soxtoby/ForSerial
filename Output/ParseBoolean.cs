namespace json
{
    public abstract class ParseBoolean : ParseValue
    {
        protected readonly bool value;

        protected ParseBoolean(bool value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddBoolean(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddBoolean(value);
        }

        public abstract ParseObject AsObject();
    }
}