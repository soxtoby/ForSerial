using System.Reflection;

namespace json.Objects
{
    public interface ObjectInterfaceProvider
    {
        GetMethod GetPropertyGetter(PropertyInfo property);
        SetMethod GetPropertySetter(PropertyInfo property);
        Method GetMethod(MethodInfo method);
        Constructor GetConstructor(ConstructorInfo constructor);
    }

    public delegate object GetMethod(object source);
    public delegate void SetMethod(object target, object value);
    public delegate object Method(object target, object[] args);
    public delegate object Constructor(object[] args);
}