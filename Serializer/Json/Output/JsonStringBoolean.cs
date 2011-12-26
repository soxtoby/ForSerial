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
                JsonStringObject obj = new JsonStringObject();
                obj.AddBoolean("value", value);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonStringObject)obj).AddBoolean(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonStringArray)array).AddBoolean(value);
            }
        }
    }
}