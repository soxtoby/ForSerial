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

        public virtual OutputStructure BeginStructure(Writer valueFactory)
        {
            return valueFactory.BeginStructure();
        }

        public virtual SequenceOutput BeginSequence(Writer valueFactory)
        {
            return valueFactory.BeginSequence();
        }

        public virtual void EndStructure(Writer writer)
        {
            writer.EndStructure();
        }

        public virtual void EndSequence(Writer writer)
        {
            writer.EndSequence();
        }
    }
}