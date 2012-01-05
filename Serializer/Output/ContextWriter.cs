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
        public abstract OutputStructure CreateStructure();
        public abstract SequenceOutput CreateSequence();

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return baseFactory.CreateReference(outputStructure);
        }
    }
}