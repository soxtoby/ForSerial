namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringNumber : NumericOutput
        {
            public JsonStringNumber(double value) : base(value) { }

            public override OutputStructure AsStructure()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddRegularProperty("value", value);
                return obj;
            }

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddRegularProperty(name, value);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddRegularValue(value);
            }
        }
    }
}