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

        public JsonStringBuilder(Options options = Options.Default)
        {
            this.options = options;
        }

        private static JsonStringBuilder defaultInstance;

        public static JsonStringBuilder Default
        {
            get { return defaultInstance ?? (defaultInstance = new JsonStringBuilder()); }
        }

        public static string GetResult(OutputStructure obj)
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
                case TypeCodeType.Object:
                    return null;

                case TypeCodeType.Boolean:
                    return (bool)value
                        ? JsonStringBoolean.True
                        : JsonStringBoolean.False;

                case TypeCodeType.String:
                    return new JsonStringString((string)value);

                case TypeCodeType.Number:
                    return new JsonStringNumber(System.Convert.ToDouble(value));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual OutputStructure CreateStructure()
        {
            JsonStringObject newObject = new JsonStringObject();
            objectReferences[newObject] = currentReferenceId++;
            return newObject;
        }

        public SequenceOutput CreateSequence()
        {
            return new JsonStringArray();
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return MaintainObjectReferences
                ? (OutputStructure)new JsonStringObjectReference(objectReferences[outputStructure])
                : new JsonStringObject();
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

        public override OutputStructure CreateStructure()
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

