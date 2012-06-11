using System;

namespace ForSerial.Objects.TypeDefinitions
{
    internal class GuidDefinition : TypeDefinition
    {
        private GuidDefinition() : base(typeof(Guid)) { }

        public static readonly GuidDefinition Instance = new GuidDefinition();

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            Guid? guid = input as Guid?;
            if (guid == null)
                writer.WriteNull();
            else
                writer.Write(guid.ToString());
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(new Guid(value.ToString()));
        }
    }
}
