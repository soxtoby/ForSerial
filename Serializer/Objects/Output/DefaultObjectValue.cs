﻿using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class DefaultObjectValue : ObjectOutput
    {
        private readonly object value;

        public DefaultObjectValue(object value)
        {
            this.value = value;
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, value);
        }

        public object GetTypedValue()
        {
            return value;
        }

        public TypeDefinition TypeDef
        {
            get
            {
                return value == null ? NullTypeDefinition.Instance
                    : TypeCache.GetTypeDefinition(value.GetType());
            }
        }
    }
}