using System;

namespace json.Objects
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

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            Guid? guid = input as Guid?;
            return guid == null
                ? null
                : valueFactory.CreateValue(guid.ToString());
        }

        public override Output CreateValue(object value)
        {
            return new TypedObjectOutputStructure(new Guid(value.ToString()));
        }
    }
}
