namespace json
{
    public abstract class ParseNumber : ParseValue
    {
        protected readonly double value;

        protected ParseNumber(double value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddNumber(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddNumber(value);
        }

        public abstract ParseObject AsObject();
    }
}