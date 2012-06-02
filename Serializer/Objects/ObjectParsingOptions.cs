using System;

namespace ForSerial.Objects
{
    public class ObjectParsingOptions
    {
        public ObjectParsingOptions()
        {
            MemberAccessibility = MemberAccessibility.PublicGet;
            MemberType = MemberType.Property;
        }

        public TypeInformationLevel SerializeTypeInformation { get; set; }
        public MemberAccessibility MemberAccessibility { get; set; }
        public MemberType MemberType { get; set; }
    }

    public enum TypeInformationLevel
    {
        Minimal,
        All,
        None
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
