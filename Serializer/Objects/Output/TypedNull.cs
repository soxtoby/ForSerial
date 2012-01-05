namespace json.Objects
{
    internal class TypedNull : NullOutput
    {
        private TypedNull() { }

        private static TypedNull value;
        public static TypedNull Value
        {
            get { return value = value ?? new TypedNull(); }
        }

        public override OutputStructure AsStructure()
        {
            return new TypedObjectOutputStructure((object)null);
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((TypedObjectOutputStructure)structure).AddProperty(name, new TypedPrimitiveValue(null));
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((TypedSequence)sequence).AddItem(null);
        }
    }
}
