namespace ForSerial.Objects
{
    public class DoNotMaintainReferencesAttribute : OverrideOptionsAttribute
    {
        public DoNotMaintainReferencesAttribute()
            : base(new PartialOptions { MaintainReferences = false })
        {
        }
    }
}