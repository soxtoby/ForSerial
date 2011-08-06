using System;

namespace json.Objects
{
    public interface TypeHandler
    {
        string GetTypeIdentifier(Type type);
        TypeDefinition GetTypeDefinition(string typeIdentifier);
        TypeDefinition GetTypeDefinition(Type type);
    }

    public class CurrentTypeHandler
    {
        [ThreadStatic]
        private static TypeHandler current;

        private static TypeHandler Current
        {
            get { return current ?? DefaultTypeHandler.Instance; }
            set { current = value; }
        }

        public static string GetTypeIdentifier(Type type)
        {
            return Current.GetTypeIdentifier(type);
        }

        public static TypeDefinition GetTypeDefinition(string typeIdentifier)
        {
            return Current.GetTypeDefinition(typeIdentifier);
        }

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            return Current.GetTypeDefinition(type);
        }

        public static IDisposable Override(TypeHandler handler)
        {
            return new TypeHandlerOverride(handler);
        }

        private class TypeHandlerOverride : IDisposable
        {
            public TypeHandlerOverride(TypeHandler handler)
            {
                current = handler;
            }

            public void Dispose()
            {
                current = null;
            }
        }
    }
}