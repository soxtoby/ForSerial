using System.Collections.Generic;
using System.Linq;
using System;

namespace json
{
    public class JsonObjectBuilder : ParseValueFactory
    {
        public static JsonObject GetResult(ParseObject obj)
        {
            if (obj == null)
                return null;

            JsonObjectObject objObject = obj as JsonObjectObject;
            if (objObject == null)
                throw new InvalidResultObject();

            return objObject.Object;
        }

        public ParseObject CreateObject()
        {
            return new JsonObjectObject();
        }

        public ParseArray CreateArray()
        {
            return new JsonObjectArray();
        }

        public ParseNumber CreateNumber(double value)
        {
            return new JsonObjectNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return new JsonObjectString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return value ? JsonObjectBoolean.True : JsonObjectBoolean.False;
        }

        public ParseNull CreateNull()
        {
            return JsonObjectNull.Value;
        }

        private class JsonObjectObject : ParseObjectBase
        {
            public JsonObject Object { get; private set; }

            public JsonObjectObject()
            {
                Object = new JsonObject();
            }

            public override void AddNull(string name)
            {
                Object[name] = null;
            }

            public override void AddBoolean(string name, bool value)
            {
                Object[name] = value;
            }

            public override void AddNumber(string name, double value)
            {
                Object[name] = value;
            }

            public override void AddString(string name, string value)
            {
                Object[name] = value;
            }

            public override void AddObject(string name, ParseObject value)
            {
                JsonObjectObject valueObject = value as JsonObjectObject;
                if (valueObject == null)
                    throw new JsonObjectBuilder.UnsupportedParseObject();
                Object[name] = valueObject.Object;
            }

            public override void AddArray(string name, ParseArray value)
            {
                JsonObjectArray valueArray = value as JsonObjectArray;
                if (valueArray == null)
                    throw new JsonObjectBuilder.UnsupportedParseArray();
                Object[name] = valueArray.Array;
            }

            public override string TypeIdentifier
            {
                get { return Object.TypeIdentifier; }
                set { Object.TypeIdentifier = value; }
            }
        }

        private class JsonObjectArray : ParseArrayBase
        {
            public List<object> Array { get; private set; }

            public JsonObjectArray()
            {
                Array = new List<object>();
            }

            public override void AddNull()
            {
                Append(null);
            }

            public override void AddBoolean(bool value)
            {
                Append(value);
            }

            public override void AddNumber(double value)
            {
                Append(value);
            }

            public override void AddString(string value)
            {
                Append(value);
            }

            public override void AddObject(ParseObject value)
            {
                JsonObjectObject valueObject = value as JsonObjectObject;
                if (valueObject == null)
                    throw new JsonObjectBuilder.UnsupportedParseObject();
                Append(valueObject.Object);
            }

            public override void AddArray(ParseArray value)
            {
                JsonObjectArray valueArray = value as JsonObjectArray;
                if (valueArray == null)
                    throw new JsonObjectBuilder.UnsupportedParseArray();
                Append(valueArray.Array);
            }

            private void Append(object value)
            {
                Array.Insert(Array.Count, value);
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonObjectObject();
                obj.AddArray("items", this);
                return obj;
            }
        }

        private class JsonObjectNull : ParseNull
        {
            private JsonObjectNull() { }

            private static JsonObjectNull value;
            public static JsonObjectNull Value
            {
                get { return value = value ?? new JsonObjectNull(); }
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonObjectObject();
                obj.AddNull("value");
                return obj;
            }
        }

        private class JsonObjectBoolean : ParseBoolean
        {
            private JsonObjectBoolean(bool value) : base(value) { }

            private static JsonObjectBoolean trueValue;
            public static JsonObjectBoolean True
            {
                get { return trueValue = trueValue ?? new JsonObjectBoolean(true); }
            }

            private static JsonObjectBoolean falseValue;
            public static JsonObjectBoolean False
            {
                get { return falseValue = falseValue ?? new JsonObjectBoolean(false); }
            }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonObjectObject();
                obj.AddBoolean("value", value);
                return obj;
            }
        }

        private class JsonObjectNumber : ParseNumber
        {
            public JsonObjectNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonObjectObject();
                obj.AddNumber("value", value);
                return obj;
            }
        }

        private class JsonObjectString : ParseString
        {
            public JsonObjectString(string value) : base(value) { }

            public override ParseObject AsObject()
            {
                ParseObject obj = new JsonObjectObject();
                obj.AddString("value", value);
                return obj;
            }
        }

        private class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects of type JsonObjectObject.") { }
        }

        private class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type JsonObjectArray.") { }
        }

        private class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a JsonObjectBuilder.") { }
        }
    }

    public class JsonObject : Dictionary<string, object>
    {
        public string TypeIdentifier { get; set; }
    }
}
