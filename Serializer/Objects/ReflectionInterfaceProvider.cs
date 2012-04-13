using System.Reflection;

namespace ForSerial.Objects
{
    public class ReflectionInterfaceProvider : ObjectInterfaceProvider
    {
        public GetMethod GetPropertyGetter(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod(true);
            return s => getMethod.Invoke(s, new object[] { });
        }

        public SetMethod GetPropertySetter(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod(true);
            return (t, v) => setMethod.Invoke(t, new[] { v });
        }

        public StaticFuncMethod GetStaticFunc(MethodInfo method)
        {
            return args => method.Invoke(null, args);
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