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

        public void AddNumber(double value)
        {
            AppendComma();
            json.Append(value);
        }

        public void AddString(string value)
        {
            AppendComma();
            json.Append('"').Append(value).Append('"');
        }

        public void AddObject(ParseObject value)
        {
            AppendComma();
            json.Append(value);
        }

        public void AddArray(ParseArray value)
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
        private double value;

        public JsonNumber(double value)
        {
            this.value = value;
        }

        public void AddToObject(ParseObject obj, string name)
        {
            obj.AddNumber(name, value);
        }

        public void AddToArray(ParseArray array)
        {
            array.AddNumber(value);
        }

        public ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddNumber("value", value);
            return obj;
        }
    }

    public class JsonString : ParseString
    {
        private string value;

        public JsonString(string value)
        {
            this.value = value;
        }

        public void AddToObject (ParseObject obj, string name)
        {
           obj.AddString(name, value);
        }

        public void AddToArray (ParseArray array)
        {
           array.AddString(value);
        }

        public ParseObject AsObject()
        {
            ParseObject obj = new JsonObject();
            obj.AddString("value", value);
            return obj;
        }
    }
}

