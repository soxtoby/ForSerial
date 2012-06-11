namespace ForSerial.Objects
{
    public struct PartialOptions
    {
        public static readonly PartialOptions Default = default(PartialOptions);

        public TypeInformationLevel? SerializeTypeInformation { get; set; }
        public MemberAccessibility? MemberAccessibility { get; set; }
        public MemberType? MemberType { get; set; }
        public bool? MaintainReferences { get; set; }
    }
}