using System;
using System.Collections.Generic;
using json.Objects;

namespace json.Json
{
    public partial class JsonStringBuilder : Writer
    {
        private readonly Options options;
        private readonly Dictionary<OutputStructure, uint> objectReferences = new Dictionary<OutputStructure, uint>();
        private uint currentReferenceId;
        private readonly Stack<Output> outputs = new Stack<Output>();

        public JsonStringBuilder(Options options = Options.Default)
        {
            this.options = options;
        }

        public static JsonStringBuilder GetDefault()
        {
            return new JsonStringBuilder();
        }

        public static string GetResult(Output obj)
        {
            if (obj == null)
                return null;

            JsonStringObject stringObj = obj as JsonStringObject;
            if (stringObj == null)
                throw new InvalidResultObject();

            return stringObj.ToString();
        }

        public Output CreateValue(object value)
        {
            if (value == null)
                return JsonStringNull.Value;

            switch (value.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Boolean:
                    return (bool)value
                        ? JsonStringBoolean.True
                        : JsonStringBoolean.False;

                case TypeCodeType.String:
                    return new JsonStringString((string)value);

                case TypeCodeType.Number:
                    return new JsonStringNumber(System.Convert.ToDouble(value));

                default:
                    return null;
            }
        }

        public OutputStructure BeginStructure()
        {
            JsonStringObject newObject = CreateJsonStringStructure();
            outputs.Push(newObject);
            objectReferences[newObject] = currentReferenceId++;
            return newObject;
        }

        protected virtual JsonStringObject CreateJsonStringStructure()
        {
            return new JsonStringObject();
        }

        public SequenceOutput BeginSequence()
        {
            JsonStringArray newArray = new JsonStringArray();
            outputs.Push(newArray);
            return newArray;
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return MaintainObjectReferences
                ? (OutputStructure)new JsonStringObjectReference(objectReferences[outputStructure])
                : new JsonStringObject();
        }

        public void EndStructure()
        {
            JsonStringObject obj = (JsonStringObject)outputs.Pop();// TODO throw exception if not object
            obj.EndStructure();
        }

        public void EndSequence()
        {
            JsonStringArray array = (JsonStringArray)outputs.Pop(); // TODO throw exception if not array
            array.EndSequence();
        }

        private bool MaintainObjectReferences
        {
            get { return (options & Options.MaintainObjectReferences) != 0; }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject()
                : base("Invalid OutputStructure type. Object must be constructed using a JsonStringBuilder.")
            {
            }
        }

        [Flags]
        public enum Options
        {
            Default = 0,
            MaintainObjectReferences = 1,
        }
    }

    public class TypedJsonStringBuilder : JsonStringBuilder
    {
        private TypedJsonStringBuilder() { }

        private static TypedJsonStringBuilder instance;
        public static TypedJsonStringBuilder Instance
        {
            get { return instance ?? (instance = new TypedJsonStringBuilder()); }
        }

        protected override JsonStringObject CreateJsonStringStructure()
        {
            return new TypedJsonStringObject();
        }

        private class TypedJsonStringObject : JsonStringObject
        {
            public override bool SetType(string typeIdentifier, Reader reader)
            {
                AddString("_type", typeIdentifier);
                return false;
            }
        }
    }
}

