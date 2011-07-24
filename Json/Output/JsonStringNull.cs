namespace json.Json
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
                JsonStringObject obj = new JsonStringObject();
                obj.AddNull("value");
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonStringObject)obj).AddNull(name);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonStringArray)array).AddNull();
            }
        }
    }
}