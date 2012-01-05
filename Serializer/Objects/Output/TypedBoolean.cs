namespace json.Objects
{
    internal class TypedBoolean : BooleanOutput
    {
        private TypedBoolean(bool value) : base(value) { }

        private static TypedBoolean trueValue;
        public static TypedBoolean True
        {
            get { return trueValue = trueValue ?? new TypedBoolean(true); }
        }

        private static TypedBoolean falseValue;
        public static TypedBoolean False
        {
            get { return falseValue = falseValue ?? new TypedBoolean(false); }
        }

        public override OutputStructure AsStructure()
        {
            return new TypedObjectOutputStructure(value);
        }

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
