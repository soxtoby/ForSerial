using System;
using System.Reflection;

namespace json.Objects
{
    public class PropertyDefinition
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool IsSerializable { get; private set; }

        private readonly MethodInfo getter;
        private readonly MethodInfo setter;
        private readonly TypeCode typeCode;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public PropertyDefinition(PropertyInfo property)
        {
            Name = property.Name;
            Type = property.PropertyType;
            getter = property.GetGetMethod();
            setter = property.GetSetMethod();
            typeCode = Type.GetTypeCode(Type);

            IsSerializable = CanGet && TypeDefinition.GetTypeDefinition(Type).IsSerializable;
        }

        public object GetFrom(object obj)
        {
            return getter.Invoke(obj, new object[] { });
        }

        public void SetOn(object obj, object value)
        {
            if (CanSet)
            {
                // Ensures the correct number type, of which there are way too many
                if (typeCode != TypeCode.Object)
                    value = Convert.ChangeType(value, typeCode);

                setter.Invoke(obj, new[] { value });
            }
        }
    }
}
