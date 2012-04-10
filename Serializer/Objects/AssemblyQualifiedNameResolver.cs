using System;

namespace json.Objects
{
    public class AssemblyQualifiedNameResolver : TypeResolver
    {
        private AssemblyQualifiedNameResolver() { }

        public static readonly AssemblyQualifiedNameResolver Instance = new AssemblyQualifiedNameResolver();

        public string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public Type GetType(string identifier)
        {
            return Type.GetType(identifier);
        }
    }
}