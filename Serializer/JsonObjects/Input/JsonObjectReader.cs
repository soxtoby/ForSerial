using System;
using System.Collections;
using System.Collections.Generic;
using json.Objects;

namespace json.JsonObjects
{
    public class JsonObjectReader : Reader
    {
        private const string TypeKey = "_type";
        private JsonObject currentObject;
        private readonly Dictionary<JsonObject, OutputStructure> objectReferences = new Dictionary<JsonObject, OutputStructure>();

        private JsonObjectReader(Writer writer) : base(writer) { }

        public static OutputStructure Read(JsonObject obj, Writer valueFactory)
        {
            JsonObjectReader reader = new JsonObjectReader(valueFactory);
            return reader.ReadObject(obj);
        }

        public override Output ReadSubStructure(Writer subWriter)
        {
            return Read(currentObject, subWriter);
        }

        private Output ReadValue(object input)
        {
            if (input == null)
                return writer.Current.CreateValue(null);

            switch (input.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    JsonObject jsonObject = input as JsonObject;
                    IEnumerable enumerable;
                    if (jsonObject != null)
                        return ReadObject(jsonObject);
                    if ((enumerable = input as IEnumerable) != null)
                        return ReadArray(enumerable);

                    throw new InvalidObject(input.GetType());

                case TypeCodeType.Boolean:
                case TypeCodeType.String:
                case TypeCodeType.Number:
                    return writer.Current.CreateValue(input);

                default:
                    throw new UnknownTypeCode(input);
            }
        }

        private OutputStructure ReadObject(JsonObject obj)
        {
            OutputStructure existingReference = objectReferences.Get(obj);
            return existingReference == null
                ? ReadNewObject(obj)
                : ReferenceObject(existingReference);
        }

        private OutputStructure ReadNewObject(JsonObject obj)
        {
            currentObject = obj;

            OutputStructure outputStructure = objectReferences[obj] = writer.Current.BeginStructure();

            foreach (var property in obj)
            {
                string name = property.Key;
                object value = property.Value;
                using (UseObjectPropertyContext(outputStructure, name))
                {
                    if (name == TypeKey)
                        outputStructure.SetType((string)obj[TypeKey], this);
                    else
                        ReadValue(value).AddToStructure(outputStructure, name);
                }
            }

            writer.Current.EndStructure();

            return outputStructure;
        }

        private OutputStructure ReferenceObject(OutputStructure existingReference)
        {
            return writer.Current.CreateReference(existingReference);
        }

        private SequenceOutput ReadArray(IEnumerable enumerable)
        {
            SequenceOutput array = writer.Current.BeginSequence();

            using (UseArrayContext(array))
            {
                foreach (object item in enumerable)
                    ReadValue(item).AddToSequence(array);
            }

            writer.Current.EndSequence();

            return array;
        }

        internal class InvalidObject : Exception
        {
            public InvalidObject(Type objectType) : base("Cannot parse object of type {0}.".FormatWith(objectType.FullName)) { }
        }

        internal class UnknownTypeCode : Exception
        {
            public UnknownTypeCode(object obj)
                : base("Type {0} has unknown TypeCode.".FormatWith(obj.GetType().FullName))
            { }
        }
    }
}
