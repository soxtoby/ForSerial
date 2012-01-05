namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringNull : NullOutput
        {
            private JsonStringNull() { }

            private static JsonStringNull value;
            public static JsonStringNull Value
            {
                get { return value = value ?? new JsonStringNull(); }
            }

            public override OutputStructure AsStructure()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddNull("value");
                return obj;
            }

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddNull(name);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddNull();
            }
        }
    }
}