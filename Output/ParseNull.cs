namespace json
{
    public abstract class ParseNull : ParseValue
    {
        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddNull(name);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddNull();
        }

        public abstract ParseObject AsObject();
    }
}