namespace json.Objects
{
    public class NullTypedWriter : Writer
    {
        private NullTypedWriter() { }

        public static readonly NullTypedWriter Instance = new NullTypedWriter();

        public Output CreateValue(object value)
        {
            return NullNullOutput.Value;
        }

        public OutputStructure CreateStructure()
        {
            return NullTypedObject.Instance.CreateStructure(null);
        }

        public SequenceOutput CreateSequence()
        {
            return TypedNullArray.Instance;
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return CreateStructure();
        }
    }
}