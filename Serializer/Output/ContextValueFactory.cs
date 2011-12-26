namespace json
{
    public abstract class ContextValueFactory : ParseValueFactory
    {
        protected readonly ParseValueFactory baseFactory;

        protected ContextValueFactory(ParseValueFactory baseFactory)
        {
            this.baseFactory = baseFactory;
        }

        public virtual ParseValue CreateValue(object value)
        {
            return baseFactory.CreateValue(value);
        }

        public virtual ParseObject CreateObject()
        {
            return baseFactory.CreateObject();
        }

        public virtual ParseArray CreateArray()
        {
            return baseFactory.CreateArray();
        }

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return baseFactory.CreateReference(parseObject);
        }
    }
}