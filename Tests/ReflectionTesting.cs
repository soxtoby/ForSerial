using System;
using System.Reflection;
using System.Reflection.Emit;

namespace json.Tests
{
    internal class ReflectionTesting
    {
        public static Type BuildTestAssembly(Action<TypeBuilder> buildMethod)
        {
            AssemblyName assemblyName = new AssemblyName("ILTest");
            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, "iltest.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Foo.Bar", TypeAttributes.Public | TypeAttributes.Class);

            buildMethod(typeBuilder);

            Type type = typeBuilder.CreateType();
            assemblyBuilder.Save("iltest.dll");
            return type;
        }
    }
}
