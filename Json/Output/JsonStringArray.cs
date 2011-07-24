using System.Text;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringArray : ParseArrayBase
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

            private void AppendComma()
            {
                if (isFirstItem)
                    isFirstItem = false;
                else
                    json.Append(',');
            }

            public override ParseObject AsObject()
            {
                JsonStringObject obj = new JsonStringObject();
                obj.AddRegularProperty("items", this);
                return obj;
            }

            public override string ToString()
            {
                return json + "]";
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonStringObject)obj).AddRegularProperty(name, this);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonStringArray)array).AddRegularValue(this);
            }
        }
    }
}