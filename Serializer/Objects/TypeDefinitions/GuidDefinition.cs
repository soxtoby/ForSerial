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

        public override bool IsDeserializable
        {
            get { return true; }
        }

        public override void ReadObject(object input, ObjectReader reader, NewWriter writer)
        {
            Guid? guid = input as Guid?;
            writer.Write(guid == null
                ? null
                : guid.ToString());
        }

        public override Output CreateValue(object value)
        {
            return new TypedObjectOutputStructure(new Guid(value.ToString()));
        }
    }
}
