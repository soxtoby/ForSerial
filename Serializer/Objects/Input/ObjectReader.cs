using System;
using System.Collections.Generic;

namespace json.Objects
{
    public partial class ObjectReader : Reader
    {
        private readonly ObjectParsingOptions options;
        private object currentObject;
        private readonly StateStack<ReaderWriter> readerWriter;
        private readonly Dictionary<object, OutputStructure> objectReferences = new Dictionary<object, OutputStructure>(new ReferenceEqualityComparer<object>());

        private ObjectReader(Writer writer, ObjectParsingOptions options)
            : base(writer)
        {
            this.options = options;
            readerWriter = new StateStack<ReaderWriter>(new ObjectReaderWriter(this));
        }

        public static Output Read(object obj, Writer valueFactory, ObjectParsingOptions options = null)
        {
            ObjectReader reader = new ObjectReader(valueFactory, options ?? new ObjectParsingOptions());

            return reader.ReadValue(obj);
        }

        public override Output ReadSubStructure(Writer subWriter)
        {
            return Read(currentObject, subWriter, options).AsStructure();
        }

        private Output ReadValue(object input)
        {
            if (input == null)
                return writer.Current.CreateValue(null);

            Type inputType = input.GetType();

            Output output = null;

            if (IsValueType(inputType))
                output = writer.Current.CreateValue(input);

            if (output == null)
            {
                OutputStructure previouslyParsedObject = objectReferences.Get(input);
                output = previouslyParsedObject == null
                    ? ReadObject(input)
                    : ReferenceObject(previouslyParsedObject);
            }

            return output;
        }

        private static bool IsValueType(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        private Output ReadObject(object input)
        {
            currentObject = input;
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(input.GetType());
            return typeDef.ReadObject(input, readerWriter.Current);
        }

        private OutputStructure ReferenceObject(OutputStructure outputStructure)
        {
            return writer.Current.CreateReference(outputStructure);
        }
    }
}

