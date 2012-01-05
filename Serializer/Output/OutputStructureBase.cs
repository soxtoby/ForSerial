namespace json
{
    public abstract class OutputStructureBase : OutputStructure
    {
        public abstract void AddToStructure(OutputStructure structure, string name);

        public abstract void AddToSequence(SequenceOutput sequence);

        public OutputStructure AsStructure()
        {
            return this;
        }

        public virtual bool SetType(string typeIdentifier, Reader reader) { return false; }

        public virtual Output CreateValue(string name, Writer valueFactory, object value)
        {
            return valueFactory.CreateValue(value);
        }

        public virtual OutputStructure CreateStructure(string name, Writer valueFactory)
        {
            return valueFactory.CreateStructure();
        }

        public virtual SequenceOutput CreateSequence(string name, Writer valueFactory)
        {
            return valueFactory.CreateSequence();
        }
    }
}