namespace json
{
    public interface Output
    {
        void AddToStructure(OutputStructure structure, string name);
        void AddToSequence(SequenceOutput sequence);
        /// <summary>
        /// Converts the current value into a <see cref="OutputStructure"/>, so parser output is consistent.
        /// </summary>
        OutputStructure AsStructure();
    }
}

