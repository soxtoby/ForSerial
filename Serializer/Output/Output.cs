namespace json
{
    public interface Output
    {
        void AddToStructure(OutputStructure structure, string name);
        void AddToSequence(SequenceOutput sequence);
    }
}

