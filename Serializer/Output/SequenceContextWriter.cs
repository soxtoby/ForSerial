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

        public override OutputStructure BeginStructure()
        {
            return sequence.BeginStructure(baseFactory);
        }

        public override SequenceOutput BeginSequence()
        {
            return sequence.BeginSequence(baseFactory);
        }

        public override void EndStructure()
        {
            sequence.EndStructure(baseFactory);
        }

        public override void EndSequence()
        {
            sequence.EndSequence(baseFactory);
        }
    }
}