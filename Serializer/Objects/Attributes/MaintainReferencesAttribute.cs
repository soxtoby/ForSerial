namespace ForSerial.Objects
{
    public class MaintainReferencesAttribute : OverrideOptionsAttribute
    {
        public MaintainReferencesAttribute()
            : base(new PartialOptions { MaintainReferences = true })
        {
        }
    }
}