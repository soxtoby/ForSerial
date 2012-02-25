using System.Text;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringArray : SequenceOutputBase
        {
            private readonly StringBuilder json = new StringBuilder();
            private bool isFirstItem = true;

            public JsonStringArray()
            {
                json.Append('[');
            }

            public void AddNull()
            {
                AddRegularValue("null");
            }

            public void AddBoolean(bool value)
            {
                AddRegularValue(value ? "true" : "false");
            }

            public void AddString(string value)
            {
                AppendComma();
                json.Append('"').Append(value).Append('"');
            }

            internal void AddRegularValue(object value)
            {
                AppendComma();
                json.Append(value);
            }

            public void EndSequence()
            {
                json.Append("]");
            }

            private void AppendComma()
            {
                if (isFirstItem)
                    isFirstItem = false;
                else
                    json.Append(',');
            }

            public override OutputStructure AsStructure()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddRegularProperty("items", this);
                obj.EndStructure();
                return obj;
            }

            public override string ToString()
            {
                return json.ToString();
            }

            public override void AddToStructure(OutputStructure structure, string name)
            {
                ((JsonStringObject)structure).AddRegularProperty(name, this);
            }

            public override void AddToSequence(SequenceOutput sequence)
            {
                ((JsonStringArray)sequence).AddRegularValue(this);
            }
        }
    }
}