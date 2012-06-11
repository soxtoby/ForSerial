using System;

namespace ForSerial.Objects
{
    public class SerializeTypeAttribute : OverrideOptionsAttribute
    {
        public SerializeTypeAttribute()
            : base(new PartialOptions { SerializeTypeInformation = TypeInformationLevel.Minimal })
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeKeyTypeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeValueTypeAttribute : Attribute { }
}
