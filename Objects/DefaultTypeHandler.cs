using System;

namespace json.Objects
{
    public class DefaultTypeHandler : TypeHandler
    {
        private DefaultTypeHandler() { }

        private static DefaultTypeHandler instance;
        public static DefaultTypeHandler Instance
        {
            get { return instance ?? (instance = new DefaultTypeHandler()); }
        }

        public string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public TypeDefinition GetTypeDefinition(Type type)
        {
            return TypeDefinition.GetTypeDefinition(type);
        }
    }
}