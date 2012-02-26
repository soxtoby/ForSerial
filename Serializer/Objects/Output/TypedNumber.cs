namespace json.Objects
{
    internal class TypedNumber : NumericOutput
    {
        public TypedNumber(double value) : base(value) { }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((TypedObjectOutputStructure)structure).AddProperty(name, new TypedPrimitiveValue(value));
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((TypedSequence)sequence).AddItem(value);
        }
    }
}
