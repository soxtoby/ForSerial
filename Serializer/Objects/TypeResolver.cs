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
        private static StateStack<TypeResolver> threadResolvers;
        private static StateStack<TypeResolver> Resolvers
        {
            get { return threadResolvers ?? (threadResolvers = new StateStack<TypeResolver>(AssemblyQualifiedNameResolver.Instance)); }
        }

        private static TypeResolver Current
        {
            get { return Resolvers.Current; }
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
            return Resolvers.OverrideState(resolver);
        }
    }
}