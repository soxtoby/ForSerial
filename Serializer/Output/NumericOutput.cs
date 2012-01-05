namespace json
{
    public abstract class NumericOutput : Output
    {
        protected readonly double value;

        protected NumericOutput(double value)
        {
            this.value = value;
        }

        public abstract void AddToStructure(OutputStructure structure, string name);
        public abstract void AddToSequence(SequenceOutput sequence);
        public abstract OutputStructure AsStructure();
    }
}