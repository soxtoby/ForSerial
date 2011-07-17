namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringNumber : ParseNumber
        {
            public JsonStringNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddNumber("value", value);
                return obj;
            }
        }
    }
}