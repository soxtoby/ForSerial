namespace json.Objects
{
    public interface TypedSequence : SequenceOutput, TypedValue
    {
        void AddItem(object item);
    }

    internal static class TypedSequenceExtensions
    {
        public static TypedSequence GetArrayAsTypedObjectArray(this SequenceOutput value)
        {
            TypedSequence arrayValue = value as TypedSequence;

            if (arrayValue == null)
                throw new TypedObjectBuilder.UnsupportedSequenceOutput();

            return arrayValue;
        }
    }
}