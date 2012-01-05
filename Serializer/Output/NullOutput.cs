namespace json
{
    public abstract class NullOutput : Output
    {
        public abstract void AddToStructure(OutputStructure structure, string name);
        public abstract void AddToSequence(SequenceOutput sequence);
        public abstract OutputStructure AsStructure();
    }
}