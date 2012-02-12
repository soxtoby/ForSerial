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

        public Method GetMethod(MethodInfo method)
        {
            return (o, args) => method.Invoke(o, args);
        }

        public Constructor GetConstructor(ConstructorInfo constructor)
        {
            return args => constructor.Invoke(args);
        }
    }
}