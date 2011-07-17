namespace json
{
    public abstract class ContextValueFactory : ParseValueFactory
    {
        protected readonly ParseValueFactory baseFactory;

        protected ContextValueFactory(ParseValueFactory baseFactory)
        {
            this.baseFactory = baseFactory;
        }

        public virtual ParseObject CreateObject()
        {
            return baseFactory.CreateObject();
        }

        public virtual ParseArray CreateArray()
        {
            return baseFactory.CreateArray();
        }

        public ParseNumber CreateNumber(double value)
        {
            return baseFactory.CreateNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return baseFactory.CreateString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return baseFactory.CreateBoolean(value);
        }

        public ParseNull CreateNull()
        {
            return baseFactory.CreateNull();
        }

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return baseFactory.CreateReference(parseObject);
        }
    }
}