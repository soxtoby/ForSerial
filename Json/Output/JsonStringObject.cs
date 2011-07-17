using System.Text;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        protected class JsonStringObject : ParseObjectBase
        {
            private readonly StringBuilder json = new StringBuilder();
            private bool isFirstProperty = true;

            public JsonStringObject()
            {
                json.Append("{");
            }

            public override void AddNull(string name)
            {
                AddRegularProperty(name, "null");
            }

            public override void AddBoolean(string name, bool value)
            {
                AddRegularProperty(name, value ? "true" : "false");
            }

            public override void AddNumber(string name, double value)
            {
                AddRegularProperty(name, value);
            }

            public override void AddString(string name, string value)
            {
                AppendDelimiter();
                AppendName(name);
                json.Append('"').Append(value).Append('"');
            }

            public override void AddObject(string name, ParseObject value)
            {
                AddRegularProperty(name, value);
            }

            public override void AddArray(string name, ParseArray value)
            {
                AddRegularProperty(name, value);
            }

            private void AddRegularProperty(string name, object value)
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
        }
    }
}