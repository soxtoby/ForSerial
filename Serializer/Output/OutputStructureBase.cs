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

        public virtual OutputStructure BeginStructure(string name, Writer valueFactory)
        {
            return valueFactory.BeginStructure();
        }

        public virtual SequenceOutput BeginSequence(string name, Writer valueFactory)
        {
            return valueFactory.BeginSequence();
        }

        public virtual void EndStructure(Writer baseFactory)
        {
            baseFactory.EndStructure();
        }

        public virtual void EndSequence(Writer baseFactory)
        {
            baseFactory.EndSequence();
        }
    }
}