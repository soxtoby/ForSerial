using System.Text;

namespace json
{
    public class JsonOutput : ParseValueFactory
    {
        public ParseObject CreateObject()
        {
            return new JsonObject();
        }

        public ParseArray CreateArray()
        {
            return new JsonArray();
        }

        public ParseNumber CreateNumber(double value)
        {
            return new JsonNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return new JsonString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return value ? JsonBoolean.True : JsonBoolean.False;
        }

        public ParseNull CreateNull()
        {
            return JsonNull.Value;
        }
    }

    public class JsonObject : ParseObject
    {
        private StringBuilder json = new StringBuilder();
        private bool isFirstProperty = true;

        public JsonObject()
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

    public class JsonArray : ParseArray
    {
        private StringBuilder json = new StringBuilder();
        private bool isFirstItem = true;

        public JsonArray()
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
            ParseObject obj = new JsonObject();
            obj.AddArray("items", this);
            return obj;
        }

        public override string ToString()
        {
            return json.ToString() + "]";
        }
    }

    public class TypedJsonObject : JsonObject
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

    public class JsonNumber : ParseNumber
    {
        public JsonNumber(double value) : base(value) { }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddNumber("value", value);
            return obj;
        }
    }

    public class JsonString : ParseString
    {
        public JsonString(string value) : base(value) { }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddString("value", value);
            return obj;
        }
    }

    public class JsonBoolean : ParseBoolean
    {
        private JsonBoolean(bool value) : base(value) { }

        private static JsonBoolean trueValue;
        public static JsonBoolean True
        {
            get { return trueValue = trueValue ?? new JsonBoolean(true); }
        }

        private static JsonBoolean falseValue;
        public static JsonBoolean False
        {
            get { return falseValue = falseValue ?? new JsonBoolean(false); }
        }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddBoolean("value", value);
            return obj;
        }
    }

    public class JsonNull : ParseNull
    {
        private JsonNull() { }

        private static JsonNull value;
        public static JsonNull Value
        {
            get { return value = value ?? new JsonNull(); }
        }

        public override ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddNull("value");
            return obj;
        }
    }
}

