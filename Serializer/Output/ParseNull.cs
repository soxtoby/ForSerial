namespace json
{
    public abstract class ParseNull : ParseValue
    {
        public abstract void AddToObject(ParseObject obj, string name);
        public abstract void AddToArray(ParseArray array);
        public abstract ParseObject AsObject();
    }
}