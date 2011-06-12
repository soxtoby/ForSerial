using System.Text;

namespace json
{
    public class JsonStringBuilder : ParseValueFactory
    {
        public ParseObject CreateObject()
        {
            return new JsonStringObject();
        }

        public ParseArray CreateArray()
        {
            return new JsonStringArray();
        }

        public ParseNumber CreateNumber(double value)
        {
            return new JsonStringNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return new JsonStringString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return value ? JsonStringBoolean.True : JsonStringBoolean.False;
        }

        public ParseNull CreateNull()
        {
            return JsonStringNull.Value;
        }
    }

    public class JsonStringObject : ParseObject
    {
        private StringBuilder json = new StringBuilder();
        private bool isFirstProperty = true;

        public JsonStringObject()
        {
            json.Append("{");
        }

        public virtual string TypeIdentifier { get; set; }

        public void AddNull(string name)
        {
            AddRegularProperty(name, "null");
        }

        public void AddBoolean(string name, bool value)
        {
            AddRegularProperty(name, value ? "true" : "false");
        }

        public void AddNumber(string name, double value)
        {
            AddRegularProperty(name, value);
        }

        public void AddString(string name, string value)
        {
            AppendDelimiter();
            AppendName(name);
            json.Append('"').Append(value).Append('"');
        }

        public void AddObject(string name, ParseObject value)
        {
            AddRegularProperty(name, value);
        }

        public void AddArray(string name, ParseArray value)
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

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddObject(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddObject(this);
        }

        public ParseObject AsObject()
        {
            return this;
        }

        public override string ToString()
        {
            return json.ToString() + "}";
        }
    }

    public class JsonStringArray : ParseArray
    {
        private StringBuilder json = new StringBuilder();
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

        public void AddNumber(double value)
        {
            AddRegularValue(value);
        }

        public void AddString(string value)
        {
            AppendComma();
            json.Append('"').Append(value).Append('"');
        }

        public void AddObject(ParseObject value)
        {
            AddRegularValue(value);
        }

        public void AddArray(ParseArray value)
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

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddArray(name, this);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddArray(this);
        }

        public ParseObject AsObject()
        {
            ParseObject obj = new JsonStringObject();
            obj.AddArray("items", this);
            return obj;
        }

        public override string ToString()
        {
            return json.ToString() + "]";
        }
    }

    public class TypedJsonObject : JsonStringObject
    {
        private string typeIdentifier;
        public override string TypeIdentifier {
            get { return typeIdentifier; }
            set
            {
                typeIdentifier = value;
                AddString("_type", value);
            }
        }

    }

    public class JsonStringNumber : ParseNumber
    {
        public JsonStringNumber(double value) : base(value) { }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonStringObject();
            obj.AddNumber("value", value);
            return obj;
        }
    }

    public class JsonStringString : ParseString
    {
        public JsonStringString(string value) : base(value) { }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonStringObject();
            obj.AddString("value", value);
            return obj;
        }
    }

    public class JsonStringBoolean : ParseBoolean
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

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonStringObject();
            obj.AddBoolean("value", value);
            return obj;
        }
    }

    public class JsonStringNull : ParseNull
    {
        private JsonStringNull() { }

        private static JsonStringNull value;
        public static JsonStringNull Value
        {
            get { return value = value ?? new JsonStringNull(); }
        }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonStringObject();
            obj.AddNull("value");
            return obj;
        }
    }
}

