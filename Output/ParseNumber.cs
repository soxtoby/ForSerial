namespace json
{
    public abstract class ParseNumber : ParseValue
    {
        protected readonly double value;

        protected ParseNumber(double value)
        {
            this.value = value;
        }

        public abstract void AddToObject(ParseObject obj, string name);
        public abstract void AddToArray(ParseArray array);
        public abstract ParseObject AsObject();
    }
}