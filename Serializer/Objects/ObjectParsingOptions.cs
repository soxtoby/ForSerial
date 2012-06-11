using System;

namespace ForSerial.Objects
{
    public class ObjectParsingOptions
    {
        public ObjectParsingOptions()
        {
            SerializeTypeInformation = TypeInformationLevel.Minimal;
            MemberAccessibility = MemberAccessibility.PublicGet;
            MemberType = MemberType.Property;
            MaintainReferences = true;
        }

        public TypeInformationLevel SerializeTypeInformation { get; set; }
        public MemberAccessibility MemberAccessibility { get; set; }
        public MemberType MemberType { get; set; }

        /// <summary>
        /// True (default): Object references are maintained.
        /// False: Objects are duplicated.
        /// </summary>
        public bool MaintainReferences { get; set; }
    }

    public enum TypeInformationLevel
    {
        None,
        Minimal,
        All
    }

    [Flags]
    public enum MemberAccessibility
    {
        PublicGet = 1,
        PublicGetSet = 3,
        Private = 4,
    }

    [Flags]
    public enum MemberType
    {
        Property = 1,
        Field = 2,
        Either = 3,
    }
}
