namespace ForSerial.Objects
{
    public struct PartialOptions
    {
        public static readonly PartialOptions Default = default(PartialOptions);

        public TypeInformationLevel? SerializeTypeInformation;
        public MemberAccessibility? MemberAccessibility;
        public MemberType? MemberType;
        public bool? MaintainReferences;
        public EnumSerialization? EnumSerialization;
    }
}