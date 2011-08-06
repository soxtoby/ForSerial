using System.Reflection;

namespace json.Objects
{
    public class PropertyDefinition
    {
        public string Name { get; private set; }
        public TypeDefinition TypeDef { get; private set; }
        public bool IsSerializable { get; private set; }

        private readonly MethodInfo getter;
        private readonly MethodInfo setter;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public PropertyDefinition(PropertyInfo property)
        {
            Name = property.Name;
            TypeDef = CurrentTypeHandler.GetTypeDefinition(property.PropertyType);
            getter = property.GetGetMethod();
            setter = property.GetSetMethod();

            IsSerializable = TypeDef.PropertyCanBeSerialized(this);
        }

        public object GetFrom(object obj)
        {
            return getter.Invoke(obj, new object[] { });
        }

        public void SetOn(object obj, object value)
        {
            if (CanSet)
                setter.Invoke(obj, new[] { TypeDef.ConvertToCorrectType(value) });
        }
    }
}
