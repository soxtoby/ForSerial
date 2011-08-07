using System;

namespace json.Objects
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeTypeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeKeyTypeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeValueTypeAttribute : Attribute { }
}
