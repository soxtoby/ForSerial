using System;
using System.Collections.Generic;
using json.Objects;

namespace json.JsonObjects
{
    public class JsonObjectBuilder : ParseValueFactory
    {
        private JsonObjectBuilder() { }

        private static JsonObjectBuilder instance;
        public static JsonObjectBuilder Instance
        {
            get { return instance ?? (instance = new JsonObjectBuilder()); }
        }

        public static JsonObject GetResult(ParseObject obj)
        {
            if (obj == null)
                return null;

            JsonObjectObject objObject = obj as JsonObjectObject;
            if (objObject == null)
                throw new InvalidResultObject();

            return objObject.Object;
        }

        public ParseValue CreateValue(object value)
        {
            if (value == null)
                return JsonObjectNull.Value;

            switch (value.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    return null;
                case TypeCodeType.Boolean:
                    return (bool)value ? JsonObjectBoolean.True : JsonObjectBoolean.False;
                case TypeCodeType.String:
                    return new JsonObjectString((string)value);
                case TypeCodeType.Number:
                    return new JsonObjectNumber(Convert.ToDouble(value));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ParseObject CreateObject()
        {
            return new JsonObjectObject();
        }

        public ParseArray CreateArray()
        {
            return new JsonObjectArray();
        }

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return parseObject;
        }

        private class JsonObjectObject : ParseObjectBase
        {
            public JsonObject Object { get; private set; }

            public JsonObjectObject()
            {
                Object = new JsonObject();
            }

            public override bool SetType(string typeIdentifier, Parser parser)
            {
                Object.TypeIdentifier = typeIdentifier;
                return false;
            }

            public void AddProperty(string name, object value)
            {
                Object[name] = value;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, Object);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(Object);
            }
        }

        private class JsonObjectArray : ParseArrayBase
        {
            private readonly List<object> values;

            public JsonObjectArray()
            {
                values = new List<object>();
            }

            public void AddValue(object value)
            {
                values.Add(value);
            }

            public override ParseObject AsObject()
            {
                JsonObjectObject obj = new JsonObjectObject();
                obj.AddProperty("items", this);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, values);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(values);
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
                JsonObjectObject obj = new JsonObjectObject();
                obj.AddProperty("value", null);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, null);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(null);
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
                JsonObjectObject obj = new JsonObjectObject();
                obj.AddProperty("value", value);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(value);
            }
        }

        private class JsonObjectNumber : ParseNumber
        {
            public JsonObjectNumber(double value) : base(value) { }

            public override ParseObject AsObject()
            {
                JsonObjectObject obj = new JsonObjectObject();
                obj.AddProperty("value", value);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(value);
            }
        }

        private class JsonObjectString : ParseString
        {
            public JsonObjectString(string value) : base(value) { }

            public override ParseObject AsObject()
            {
                JsonObjectObject obj = new JsonObjectObject();
                obj.AddProperty("value", value);
                return obj;
            }

            public override void AddToObject(ParseObject obj, string name)
            {
                ((JsonObjectObject)obj).AddProperty(name, value);
            }

            public override void AddToArray(ParseArray array)
            {
                ((JsonObjectArray)array).AddValue(value);
            }
        }

        internal class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects of type JsonObjectObject.") { }
        }

        internal class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type JsonObjectArray.") { }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a JsonObjectBuilder.") { }
        }
    }
}
