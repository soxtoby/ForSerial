using System;

namespace json.Objects
{
    public interface TypeResolver
    {
        string GetTypeIdentifier(Type type);
        Type GetType(string identifier);
    }

    public static class CurrentTypeResolver
    {
        [ThreadStatic]
        private static TypeResolver current;
        private static TypeResolver Current
        {
            get { return current ?? AssemblyQualifiedNameResolver.Instance; }
        }

        public static string GetTypeIdentifier(Type type)
        {
            return Current.GetTypeIdentifier(type);
        }

        public static Type GetType(string identifier)
        {
            return Current.GetType(identifier);
        }

        public static IDisposable Override(TypeResolver resolver)
        {
            return new TypeHandlerOverride(resolver);
        }

        private class TypeHandlerOverride : IDisposable
        {
            public TypeHandlerOverride(TypeResolver resolver)
            {
                current = resolver;
            }

            public void Dispose()
            {
                current = null;
            }
        }
    }
}