using System.Text;

namespace json.Json.ValueFactory
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

            public override void AddNull()
            {
                AddRegularValue("null");
            }

            public override void AddBoolean(bool value)
            {
                AddRegularValue(value ? "true" : "false");
            }

            public override void AddNumber(double value)
            {
                AddRegularValue(value);
            }

            public override void AddString(string value)
            {
                AppendComma();
                json.Append('"').Append(value).Append('"');
            }

            public override void AddObject(ParseObject value)
            {
                AddRegularValue(value);
            }

            public override void AddArray(ParseArray value)
            {
                AddRegularValue(value);
            }

            private void AddRegularValue(object value)
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
                ParseObject obj = new JsonStringObject();
                obj.AddArray("items", this);
                return obj;
            }

            public override string ToString()
            {
                return json + "]";
            }
        }
    }
}