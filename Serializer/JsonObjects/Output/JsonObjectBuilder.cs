using System;
using System.Collections.Generic;
using json.Objects;

namespace json.JsonObjects
{
    public class JsonObjectBuilder : Writer
    {
        private JsonObjectBuilder() { }

        public static readonly JsonObjectBuilder Instance = new JsonObjectBuilder();

        private readonly Stack<Output> outputs = new Stack<Output>();

        public static object GetResult(Output obj)
        {
            if (obj == null)
                return null;

            JsonObjectOutput objObject = obj as JsonObjectOutput;
            if (objObject == null)
                throw new InvalidResultObject();

            return objObject.Object;
        }

        public Output CreateValue(object value)
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

        public OutputStructure BeginStructure()
        {
            JsonObjectObject obj = new JsonObjectObject();
            outputs.Push(obj);
            return obj;
        }

        public SequenceOutput BeginSequence()
        {
            JsonObjectArray array = new JsonObjectArray();
            outputs.Push(array);
            return array;
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return outputStructure;
        }

        public void EndStructure()
        {
            outputs.Pop();
        }

        public void EndSequence()
        {
            outputs.Pop();
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid OutputStructure type. Object must be constructed using a JsonObjectBuilder.") { }
        }
    }

    internal interface JsonObjectOutput : Output
    {
        object Object { get; }
    }

    internal class JsonObjectObject : OutputStructureBase, JsonObjectOutput
    {
        private readonly JsonObject jsonObject;

        public object Object { get { return jsonObject; } }

        public JsonObjectObject()
        {
            jsonObject = new JsonObject();
        }

        public override bool SetType(string typeIdentifier, Reader reader)
        {
            jsonObject.TypeIdentifier = typeIdentifier;
            return false;
        }

        public void AddProperty(string name, object value)
        {
            jsonObject[name] = value;
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, Object);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(Object);
        }
    }

    internal class JsonObjectArray : SequenceOutputBase, JsonObjectOutput
    {
        private readonly List<object> values;

        public object Object { get { return values; } }

        public JsonObjectArray()
        {
            values = new List<object>();
        }

        public void AddValue(object value)
        {
            values.Add(value);
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, values);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(values);
        }
    }

    internal class JsonObjectNull : NullOutput, JsonObjectOutput
    {
        private JsonObjectNull() { }

        private static JsonObjectNull value;
        public static JsonObjectNull Value
        {
            get { return value = value ?? new JsonObjectNull(); }
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, null);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(null);
        }

        public object Object
        {
            get { return null; }
        }
    }

    internal class JsonObjectBoolean : BooleanOutput, JsonObjectOutput
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

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, value);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(value);
        }

        public object Object
        {
            get { return value; }
        }
    }

    internal class JsonObjectNumber : NumericOutput, JsonObjectOutput
    {
        public JsonObjectNumber(double value) : base(value) { }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, value);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(value);
        }

        public object Object
        {
            get { return value; }
        }
    }

    internal class JsonObjectString : StringOutput, JsonObjectOutput
    {
        public JsonObjectString(string value) : base(value) { }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((JsonObjectObject)structure).AddProperty(name, value);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((JsonObjectArray)sequence).AddValue(value);
        }

        public object Object
        {
            get { return value; }
        }
    }
}
