namespace json
{
    public abstract class StringOutput : Output
    {
        protected readonly string value;

        protected StringOutput(string value)
        {
            this.value = value;
        }

        public abstract void AddToStructure(OutputStructure structure, string name);
        public abstract void AddToSequence(SequenceOutput sequence);
        public abstract OutputStructure AsStructure();
    }
}