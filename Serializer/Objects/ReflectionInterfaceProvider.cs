using System.Reflection;

namespace json.Objects
{
    public class ReflectionInterfaceProvider : ObjectInterfaceProvider
    {
        public GetMethod GetPropertyGetter(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            return getMethod == null
                       ? (GetMethod)null
                       : s => getMethod.Invoke(s, new object[] { });
        }

        public SetMethod GetPropertySetter(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            return setMethod == null
                       ? (SetMethod)null
                       : (t, v) => setMethod.Invoke(t, new[] { v });
        }

        public GetMethod GetFieldGetter(FieldInfo field)
        {
            return s => field.GetValue(s);
        }

        public SetMethod GetFieldSetter(FieldInfo field)
        {
            return (t, v) => field.SetValue(t, v);
        }
    }
}