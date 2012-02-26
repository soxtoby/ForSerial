using System;

namespace json.Objects.TypeDefinitions
{
    internal class GuidDefinition : TypeDefinition
    {
        private GuidDefinition() : base(typeof(Guid)) { }

        internal static GuidDefinition CreateGuidDefinition(Type type)
        {
            return type == typeof(Guid)
                ? new GuidDefinition()
                : null;
        }

        protected override bool ShouldWriterTypeIdentifier { get { return false; } }

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            Guid? guid = input as Guid?;
            writer.Write(guid == null
                ? null
                : guid.ToString());
        }

        public override ObjectValue CreateValue(object value)
        {
            return new DefaultObjectValue(new Guid(value.ToString()));
        }
    }
}
