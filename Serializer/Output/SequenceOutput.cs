namespace json
{
    public interface SequenceOutput : Output
    {
        Output CreateValue(Writer valueFactory, object value);
        OutputStructure BeginStructure(Writer valueFactory);
        SequenceOutput BeginSequence(Writer valueFactory);
        void EndStructure(Writer writer);
        void EndSequence(Writer writer);
    }
}