using System.Reflection;

namespace json.Objects
{
    public interface ObjectInterfaceProvider
    {
        GetMethod GetPropertyGetter(PropertyInfo property);
        SetMethod GetPropertySetter(PropertyInfo property);
        StaticFuncMethod GetStaticFunc(MethodInfo method);
        ActionMethod GetAction(MethodInfo method);
        ConstructorMethod GetConstructor(ConstructorInfo constructor);
    }

    public delegate object GetMethod(object source);
    public delegate void SetMethod(object target, object value);
    public delegate object FuncMethod(object target, object[] args);
    public delegate object StaticFuncMethod(object[] args);
    public delegate void ActionMethod(object target, object[] args);
    public delegate object ConstructorMethod(object[] args);
}