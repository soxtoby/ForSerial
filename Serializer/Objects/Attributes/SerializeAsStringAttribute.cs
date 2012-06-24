namespace ForSerial.Objects
{
    public class SerializeAsStringAttribute : OverrideOptionsAttribute
    {
        public SerializeAsStringAttribute()
            : base(new PartialOptions { EnumSerialization = EnumSerialization.AsString })
        {
        }
    }
}