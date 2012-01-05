namespace json
{
    public class SequenceContextWriter : ContextWriter
    {
        private readonly SequenceOutput sequence;

        public SequenceContextWriter(Writer baseFactory, SequenceOutput sequence)
            : base(baseFactory)
        {
            this.sequence = sequence;
        }

        public override Output CreateValue(object value)
        {
            return sequence.CreateValue(baseFactory, value);
        }

        public override OutputStructure CreateStructure()
        {
            return sequence.CreateStructure(baseFactory);
        }

        public override SequenceOutput CreateSequence()
        {
            return sequence.CreateSequence(baseFactory);
        }
    }
}