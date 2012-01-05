namespace json
{
    public interface SequenceOutput : Output
    {
        Output CreateValue(Writer valueFactory, object value);
        OutputStructure CreateStructure(Writer valueFactory);
        SequenceOutput CreateSequence(Writer valueFactory);
    }
}