namespace json
{
    public class PropertyContextWriter : ContextWriter
    {
        private readonly OutputStructure propertyOwner;
        private readonly string propertyName;

        public PropertyContextWriter(Writer baseFactory, OutputStructure propertyOwner, string propertyName)
            : base(baseFactory)
        {
            this.propertyOwner = propertyOwner;
            this.propertyName = propertyName;
        }

        public override Output CreateValue(object value)
        {
            return propertyOwner.CreateValue(propertyName, baseFactory, value);
        }

        public override OutputStructure CreateStructure()
        {
            return propertyOwner.CreateStructure(propertyName, baseFactory);
        }

        public override SequenceOutput CreateSequence()
        {
            return propertyOwner.CreateSequence(propertyName, baseFactory);
        }
    }
}