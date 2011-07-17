using System;
using System.Collections.Generic;

namespace json.Json
{
    public partial class JsonStringBuilder : ParseValueFactory
    {
        private readonly Options options;
        private readonly Dictionary<ParseObject, uint> objectReferences = new Dictionary<ParseObject, uint>();
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
            JsonStringObject newObject = new JsonStringObject();
            objectReferences[newObject] = currentReferenceId++;
            return newObject;
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

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return MaintainObjectReferences
                ? (ParseObject)new JsonStringObjectReference(objectReferences[parseObject])
                : new JsonStringObject();
        }

        private bool MaintainObjectReferences
        {
            get { return (options & Options.MaintainObjectReferences) != 0; }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject()
                : base("Invalid ParseObject type. Object must be constructed using a JsonStringBuilder.")
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

