namespace json
{
    public interface Writer
    {
        Output CreateValue(object value);
        OutputStructure BeginStructure();
        SequenceOutput BeginSequence();
        OutputStructure CreateReference(OutputStructure outputStructure);
        void EndStructure();
        void EndSequence();
    }
}