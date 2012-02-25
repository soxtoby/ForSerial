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

        public override OutputStructure BeginStructure()
        {
            return propertyOwner.BeginStructure(propertyName, baseFactory);
        }

        public override SequenceOutput BeginSequence()
        {
            return propertyOwner.BeginSequence(propertyName, baseFactory);
        }

        public override void EndStructure()
        {
            propertyOwner.EndStructure(baseFactory);
        }

        public override void EndSequence()
        {
            propertyOwner.EndSequence(baseFactory);
        }
    }
}