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

        protected override bool DetermineIfDeserializable()
        {
            return true;
        }

        public override ParseValue ParseObject(object input, ParserValueFactory valueFactory)
        {
            Guid? guid = input as Guid?;
            return guid == null
                ? null
                : valueFactory.CreateString(guid.ToString());
        }
    }
}
