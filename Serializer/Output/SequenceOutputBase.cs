namespace json
{
    public abstract class SequenceOutputBase : SequenceOutput
    {
        public abstract void AddToStructure(OutputStructure structure, string name);
        public abstract void AddToSequence(SequenceOutput sequence);
        public abstract OutputStructure AsStructure();

        public virtual Output CreateValue(Writer valueFactory, object value)
        {
            return valueFactory.CreateValue(value);
        }

        public virtual OutputStructure CreateStructure(Writer valueFactory)
        {
            return valueFactory.CreateStructure();
        }

        public virtual SequenceOutput CreateSequence(Writer valueFactory)
        {
            return valueFactory.CreateSequence();
        }
    }
}