using System;
using System.Reflection;

namespace json
{
    public class PropertyDefinition
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool IsSerializable { get; private set; }
        private MethodInfo getter;
        private MethodInfo setter;

        public PropertyDefinition(PropertyInfo property)
        {
            Name = property.Name;
            Type = property.PropertyType;
            getter = property.GetGetMethod();
            setter = property.GetSetMethod();

            IsSerializable = getter != null;
        }

        public object GetFrom(object obj)
        {
            return getter.Invoke(obj, new object[] {  });
        }

        public void SetOn(object obj, object value)
        {
            setter.Invoke(obj, new[] { value });
        }
    }
}
