using System;

namespace ForSerial.Objects
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class SerializationConstructorAttribute : Attribute
    {
    }
}