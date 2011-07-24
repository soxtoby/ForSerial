namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringNumber : ParseNumber
        {
            public JsonStringNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddRegularProperty("value", value);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonStringObject)obj).AddRegularProperty(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonStringArray)array).AddRegularValue(value);
            }
        }
    }
}