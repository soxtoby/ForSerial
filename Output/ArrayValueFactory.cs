namespace json
{
    public class ArrayValueFactory : ContextValueFactory
    {
        private readonly ParseArray array;

        public ArrayValueFactory(ParseValueFactory baseFactory, ParseArray array)
            : base(baseFactory)
        {
            this.array = array;
        }

        public override ParseObject CreateObject()
        {
            return array.CreateObject(baseFactory);
        }

        public override ParseArray CreateArray()
        {
            return array.CreateArray(baseFactory);
        }
    }
}