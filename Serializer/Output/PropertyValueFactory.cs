namespace json
{
    public class PropertyValueFactory : ContextValueFactory
    {
        private readonly ParseObject propertyOwner;
        private readonly string propertyName;

        public PropertyValueFactory(ParseValueFactory baseFactory, ParseObject propertyOwner, string propertyName)
            : base(baseFactory)
        {
            this.propertyOwner = propertyOwner;
            this.propertyName = propertyName;
        }

        public override ParseValue CreateValue(object value)
        {
            return propertyOwner.CreateValue(propertyName, baseFactory, value);
        }

        public override ParseObject CreateObject()
        {
            return propertyOwner.CreateObject(propertyName, baseFactory);
        }

        public override ParseArray CreateArray()
        {
            return propertyOwner.CreateArray(propertyName, baseFactory);
        }
    }
}