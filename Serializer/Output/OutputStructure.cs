namespace json
{
    public interface OutputStructure : Output
    {
        /// <summary>
        /// Sets the object's type.
        /// </summary>
        /// <returns>
        /// True if the object was pre-built and the Reader should skip populating it.
        /// </returns>
        bool SetType(string typeIdentifier, Reader reader);
        Output CreateValue(string name, Writer valueFactory, object value);
        OutputStructure BeginStructure(string name, Writer valueFactory);
        SequenceOutput BeginSequence(string name, Writer valueFactory);
        void EndStructure(Writer baseFactory);
        void EndSequence(Writer baseFactory);
    }
}