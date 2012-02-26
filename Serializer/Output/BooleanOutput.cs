namespace json
{
    public abstract class BooleanOutput : Output
    {
        protected readonly bool value;

        protected BooleanOutput(bool value)
        {
            this.value = value;
        }

        public abstract void AddToStructure(OutputStructure structure, string name);
        public abstract void AddToSequence(SequenceOutput sequence);
    }
}