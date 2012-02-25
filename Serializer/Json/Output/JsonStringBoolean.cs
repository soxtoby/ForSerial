namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringBoolean : BooleanOutput
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

            public override OutputStructure AsStructure()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddBoolean("value", value);
                obj.EndStructure();
                return obj;
            }

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddBoolean(name, value);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddBoolean(value);
            }
        }
    }
}