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

        public TypeInformationLevel SerializeTypeInformation;
        public MemberAccessibility MemberAccessibility;
        public MemberType MemberType;

        /// <summary>
        /// True (default): Object references are maintained.
        /// False: Objects are duplicated.
        /// </summary>
        public bool MaintainReferences;

        public EnumSerialization EnumSerialization;
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

    public enum EnumSerialization
    {
        AsInteger,
        AsString
    }
}
