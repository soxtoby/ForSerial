namespace json
{
    public interface Writer
    {
        Output CreateValue(object value);
        OutputStructure CreateStructure();
        SequenceOutput CreateSequence();
        OutputStructure CreateReference(OutputStructure outputStructure);
    }
}