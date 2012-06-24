namespace ForSerial.Objects
{
    public class SerializeAsIntegerAttribute : OverrideOptionsAttribute
    {
        public SerializeAsIntegerAttribute()
            : base(new PartialOptions { EnumSerialization = EnumSerialization.AsInteger })
        {
        }
    }
}