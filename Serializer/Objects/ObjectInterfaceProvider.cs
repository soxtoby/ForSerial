using System.Reflection;

namespace json.Objects
{
    public interface ObjectInterfaceProvider
    {
        GetMethod GetPropertyGetter(PropertyInfo property);
        SetMethod GetPropertySetter(PropertyInfo property);
    }

    public delegate object GetMethod(object source);
    public delegate void SetMethod(object target, object value);
}