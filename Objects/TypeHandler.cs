using System;

namespace json.Objects
{
    public interface TypeHandler
    {
        string GetTypeIdentifier(Type type);
        TypeDefinition GetTypeDefinition(Type type);
    }
}