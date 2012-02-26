using System;

namespace json.Objects
{
    public partial class ObjectReader
    {
        private readonly Writer writer;
        private readonly ObjectParsingOptions options;

        private ObjectReader(Writer writer, ObjectParsingOptions options)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (options == null) throw new ArgumentNullException("options");

            this.writer = writer;
            this.options = options;
        }

        public bool SerializeAllTypes
        {
            get { return options.SerializeAllTypes; }
        }

        public static void Read(object obj, Writer writer, ObjectParsingOptions options = null)
        {
            ObjectReader reader = new ObjectReader(writer, options ?? new ObjectParsingOptions());

            reader.Read(obj);
        }

        // TODO reimplement prebuild
        //public override Output ReadSubStructure(Writer subWriter)
        //{
        //    return Read(currentObject, subWriter, options).AsStructure();
        //}

        public void Read(object input)
        {
            if (writer.CanWrite(input))
                writer.Write(input);
            else
                ReadObject(input);

            // TODO reimplement object references
            //OutputStructure previouslyParsedObject = objectReferences.Get(input);
            //output = previouslyParsedObject == null
            //    ? ReadObject(input)
            //    : ReferenceObject(previouslyParsedObject);

        }

        private void ReadObject(object input)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(input.GetType());
            typeDef.ReadObject(input, this, writer);
        }

        // TODO reimplement object references
        //private OutputStructure ReferenceObject(OutputStructure outputStructure)
        //{
        //    return writer.CreateReference(outputStructure);
        //}
    }
}

