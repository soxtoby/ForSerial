using System.Reflection;

namespace json.Objects
{
    public class ReflectionInterfaceProvider : ObjectInterfaceProvider
    {
        public GetMethod GetPropertyGetter(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            return s => getMethod.Invoke(s, new object[] { });
        }

        public SetMethod GetPropertySetter(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            return (t, v) => setMethod.Invoke(t, new[] { v });
        }

        public FuncMethod GetFunc(MethodInfo method)
        {
            return (o, args) => method.Invoke(o, args);
        }

        public ActionMethod GetAction(MethodInfo method)
        {
            return (o, args) => method.Invoke(o, args);
        }

        public ConstructorMethod GetConstructor(ConstructorInfo constructor)
        {
            return args => constructor.Invoke(args);
        }
    }
}