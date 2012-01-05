using System.Text;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        protected class JsonStringObject : OutputStructureBase
        {
            private readonly StringBuilder json = new StringBuilder();
            private bool isFirstProperty = true;

            public JsonStringObject()
            {
                json.Append("{");
            }

            public void AddNull(string name)
            {
                AddRegularProperty(name, "null");
            }

            public void AddBoolean(string name, bool value)
            {
                AddRegularProperty(name, value ? "true" : "false");
            }

            public void AddString(string name, string value)
            {
                AppendDelimiter();
                AppendName(name);
                json.Append('"').Append(value).Append('"');
            }

            internal void AddRegularProperty(string name, object value)
            {
                AppendDelimiter();
                AppendName(name);
                json.Append(value);
            }

            private void AppendDelimiter()
            {
                if (isFirstProperty)
                    isFirstProperty = false;
                else
                    json.Append(',');
            }

            private void AppendName(string name)
            {
                json.Append('"').Append(name).Append("\":");
            }

            public override string ToString()
            {
                return json + "}";
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