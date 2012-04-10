using System;

namespace json.Objects
{
    public class SerializeTypeAttribute : PropertyDefinitionAttribute
    {
        public override void Read(object value, ObjectReader reader, Writer writer)
        {
            TypeDef.ReadObject(value, reader, writer, true);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeKeyTypeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeValueTypeAttribute : Attribute { }
}
