namespace json
{
    public abstract class ContextWriter : Writer
    {
        protected readonly Writer baseFactory;

        protected ContextWriter(Writer baseFactory)
        {
            this.baseFactory = baseFactory;
        }

        public abstract Output CreateValue(object value);
        public abstract OutputStructure BeginStructure();
        public abstract SequenceOutput BeginSequence();
        public abstract void EndStructure();
        public abstract void EndSequence();

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return baseFactory.CreateReference(outputStructure);
        }
    }
}