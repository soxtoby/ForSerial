namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringBoolean : ParseBoolean
        {
            private JsonStringBoolean(bool value) : base(value) { }

            private static JsonStringBoolean trueValue;
            public static JsonStringBoolean True
            {
                get { return trueValue = trueValue ?? new JsonStringBoolean(true); }
            }

            private static JsonStringBoolean falseValue;
            public static JsonStringBoolean False
            {
                get { return falseValue = falseValue ?? new JsonStringBoolean(false); }
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddBoolean("value", value);
                return obj;
            }
        }
    }
}