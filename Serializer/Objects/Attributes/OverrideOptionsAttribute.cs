namespace ForSerial.Objects
{
    public class OverrideOptionsAttribute : PropertyDefinitionAttribute
    {
        private readonly PartialOptions optionsOverride;

        public OverrideOptionsAttribute(PartialOptions optionsOverride)
        {
            this.optionsOverride = optionsOverride;
        }

        public override void Read(object value, ObjectReader reader, Writer writer)
        {
            TypeDef.ReadObject(value, reader, writer, optionsOverride);
        }
    }
}