namespace json.Json.ValueFactory
{
    public partial class JsonStringBuilder
    {
        private class JsonStringNull : ParseNull
        {
            private JsonStringNull() { }

            private static JsonStringNull value;
            public static JsonStringNull Value
            {
                get { return value = value ?? new JsonStringNull(); }
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddNull("value");
                return obj;
            }
        }
    }
}