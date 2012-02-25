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

        public OutputStructure BeginStructure()
        {
            return NullTypedObject.Instance.BeginStructure(null);
        }

        public SequenceOutput BeginSequence()
        {
            return TypedNullArray.Instance;
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return BeginStructure();
        }

        public void EndStructure()
        {
        }

        public void EndSequence()
        {
        }
    }
}