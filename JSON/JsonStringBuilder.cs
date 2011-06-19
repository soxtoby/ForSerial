using System;
using System.Text;
using System.Text.RegularExpressions;

namespace json.Json
{
    public class JsonStringBuilder : ParseValueFactory
    {
        protected JsonStringBuilder() { }

        private static JsonStringBuilder instance;
        public static JsonStringBuilder Instance
        {
            get { return instance ?? (instance = new JsonStringBuilder()); }
        }

        public static string GetResult(ParseObject obj)
        {
            if (obj == null)
                return null;

            JsonStringObject stringObj = obj as JsonStringObject;
            if (stringObj == null)
                throw new InvalidResultObject();

            return stringObj.ToString();
        }

        public virtual ParseObject CreateObject()
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
                return json.ToString() + "}";
            }
        }

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

        private class JsonStringNumber : ParseNumber
        {
            public JsonStringNumber(double value)
                : base(value)
            {
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddNumber("value", value);
                return obj;
            }
        }

        private static readonly Regex charactersToEscape = new Regex(@"[""\\]", RegexOptions.Compiled); // TODO escape control characters as well

        private class JsonStringString : ParseString
        {
            public JsonStringString(string value)
                : base(EscapeForJson(value))
            {
            }

            private static string EscapeForJson(string value)
            {
                return charactersToEscape.Replace(value, @"\$0");
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonStringObject();
                obj.AddString("value", value);
                return obj;
            }
        }

        private class JsonStringBoolean : ParseBoolean
        {
            private JsonStringBoolean(bool value)
                : base(value)
            {
            }

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

        private class JsonStringNull : ParseNull
        {
            private JsonStringNull()
            {
            }

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

        private class InvalidResultObject : Exception
        {
            public InvalidResultObject()
                : base("Invalid ParseObject type. Object must be constructed using a JsonStringBuilder.")
            {
            }
        }
    }

    public class TypedJsonStringBuilder : JsonStringBuilder
    {
        private TypedJsonStringBuilder() { }

        private static TypedJsonStringBuilder instance;
        public new static TypedJsonStringBuilder Instance
        {
            get { return instance ?? (instance = new TypedJsonStringBuilder()); }
        }

        public override ParseObject CreateObject()
        {
            return new TypedJsonStringObject();
        }

        private class TypedJsonStringObject : JsonStringObject
        {
            public override bool SetType(string typeIdentifier, Parser parser)
            {
                AddString("_type", typeIdentifier);
                return false;
            }
        }
    }
}

