using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace json.Objects
{
    public class DynamicMethodProvider : ObjectInterfaceProvider
    {
        public GetMethod GetPropertyGetter(PropertyInfo property)
        {
            GetMethod compiledGet = null;
            return source => (compiledGet ?? (compiledGet = CompilePropertyGetter(property)))(source);
        }

        private static GetMethod CompilePropertyGetter(PropertyInfo property)
        {
            const int instanceArg = 1;
            DynamicMethod dynamicGet = CreateDynamicFunc(property.DeclaringType, "dynamicGet_" + property.Name, instanceArg);
            dynamicGet.GetILGenerator()
                .LoadObjectInstance(property.DeclaringType)
                .CallMethod(property.GetGetMethod(true))
                .BoxIfNeeded(property.PropertyType)
                .Return();

            return (GetMethod)dynamicGet.CreateDelegate(typeof(GetMethod));
        }

        public SetMethod GetPropertySetter(PropertyInfo property)
        {
            SetMethod compiledSet = null;
            return (target, value) => (compiledSet ?? (compiledSet = CompilePropertySetter(property)))(target, value);
        }

        private static SetMethod CompilePropertySetter(PropertyInfo property)
        {
            const int instanceAndValueArgs = 2;
            DynamicMethod dynamicSet = CreateDynamicAction(property.DeclaringType, "dynamicSet_" + property.Name, instanceAndValueArgs);

            dynamicSet.GetILGenerator()
                .LoadObjectInstance(property.DeclaringType)
                .LoadArg(1)
                .UnboxIfNeeded(property.PropertyType)
                .CallMethod(property.GetSetMethod(true))
                .Return();

            return (SetMethod)dynamicSet.CreateDelegate(typeof(SetMethod));
        }

        public GetMethod GetFieldGetter(FieldInfo field)
        {
            GetMethod compiledGet = null;
            return source => (compiledGet ?? (compiledGet = CompileFieldGetter(field)))(source);
        }

        private static GetMethod CompileFieldGetter(FieldInfo field)
        {
            const int instanceArg = 1;
            DynamicMethod dynamicGet = CreateDynamicFunc(field.DeclaringType, "dynamicGet_" + field.Name, instanceArg);

            dynamicGet.GetILGenerator()
                .LoadObjectInstance(field.DeclaringType)
                .LoadField(field)
                .BoxIfNeeded(field.FieldType)
                .Return();

            return (GetMethod)dynamicGet.CreateDelegate(typeof(GetMethod));
        }

        public SetMethod GetFieldSetter(FieldInfo field)
        {
            SetMethod compiledSet = null;
            return (target, value) => (compiledSet ?? (compiledSet = CompileFieldSetter(field)))(target, value);
        }

        private static SetMethod CompileFieldSetter(FieldInfo field)
        {
            const int instanceAndValueArgs = 2;
            DynamicMethod dynamicSet = CreateDynamicAction(field.DeclaringType, "dynamicSet_" + field.Name, instanceAndValueArgs);

            dynamicSet.GetILGenerator()
                .LoadObjectInstance(field.DeclaringType)
                .LoadArg(1)
                .UnboxIfNeeded(field.FieldType)
                .SetField(field)
                .Return();

            return (SetMethod)dynamicSet.CreateDelegate(typeof(SetMethod));
        }

        public StaticFuncMethod GetStaticFunc(MethodInfo method)
        {
            StaticFuncMethod compiledFuncCall = null;
            return (args) => (compiledFuncCall ?? (compiledFuncCall = CompileStaticFuncCall(method)))(args);
        }

        private static StaticFuncMethod CompileStaticFuncCall(MethodInfo method)
        {
            const int objectArrayArg = 1;
            DynamicMethod dynamicMethod = CreateDynamicFunc(method.DeclaringType, "dynamicCall_" + method.Name, objectArrayArg);

            dynamicMethod.GetILGenerator()
                .LoadParamsFromObjectArrayArg(0, method)
                .CallMethod(method)
                .Return();

            return (StaticFuncMethod)dynamicMethod.CreateDelegate(typeof(StaticFuncMethod));
        }

        public ActionMethod GetAction(MethodInfo method)
        {
            ActionMethod compiledActionCall = null;
            return (target, args) => (compiledActionCall ?? (compiledActionCall = CompileActionCall(method)))(target, args);
        }

        private static ActionMethod CompileActionCall(MethodInfo method)
        {
            const int instanceAndObjectArrayArgs = 2;
            DynamicMethod dynamicMethod = CreateDynamicAction(method.DeclaringType, "dynamicCall_" + method.Name, instanceAndObjectArrayArgs);

            dynamicMethod.GetILGenerator()
                .LoadObjectInstance(method.DeclaringType)
                .LoadParamsFromObjectArrayArg(1, method)
                .CallMethod(method)
                .Return();

            return (ActionMethod)dynamicMethod.CreateDelegate(typeof(ActionMethod));
        }

        public ConstructorMethod GetConstructor(ConstructorInfo constructor)
        {
            DynamicMethod dynamicConstructor = CreateDynamicConstructor(constructor);
            ILGenerator il = dynamicConstructor.GetILGenerator();
            ParameterInfo[] parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                switch (i)
                {
                    case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                    default: il.Emit(OpCodes.Ldc_I4, i); break;
                }
                il.Emit(OpCodes.Ldelem_Ref);
                Type paramType = parameters[i].ParameterType;
                il.Emit(paramType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
            }
            il.Emit(OpCodes.Newobj, constructor);
            il.BoxIfNeeded(constructor.DeclaringType);
            il.Emit(OpCodes.Ret);

            return (ConstructorMethod)dynamicConstructor.CreateDelegate(typeof(ConstructorMethod));
        }

        private static DynamicMethod CreateDynamicFunc(Type type, string name, int argCount)
        {
            return CreateDynamicMethod(type, name, argCount, typeof(object));
        }

        private static DynamicMethod CreateDynamicAction(Type type, string name, int argCount)
        {
            return CreateDynamicMethod(type, name, argCount, typeof(void));
        }

        private static DynamicMethod CreateDynamicMethod(Type type, string name, int argCount, Type returnType)
        {
            return new DynamicMethod(name, returnType, Enumerable.Repeat(typeof(object), argCount).ToArray(), type, true);
        }

        private static DynamicMethod CreateDynamicConstructor(ConstructorInfo constructor)
        {
            Type type = constructor.DeclaringType;
            string methodName = "construct_" + type.Name;
            Type returnType = typeof(object);
            Type[] methodParameterTypes = new[] { typeof(object[]) };

            return type.IsPrimitive || type.IsArray    // FIXME primitives and arrays simply shouldn't get getting in here
                ? new DynamicMethod(methodName, returnType, methodParameterTypes)
                : new DynamicMethod(methodName, returnType, methodParameterTypes, type, true);
        }
    }

    internal static class IlGeneratorExtensions
    {
        public static ILGenerator LoadObjectInstance(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldarg_0);

            if (type.IsValueType)
                il.UnboxAndLoadValue(type);

            return il;
        }

        private static void UnboxAndLoadValue(this ILGenerator il, Type type)
        {
            il.DeclareLocal(type);
            il.Emit(OpCodes.Unbox_Any, type);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloca_S, 0);
        }

        public static ILGenerator LoadParamsFromObjectArrayArg(this ILGenerator il, int objectArrayIndex, MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                il.LoadArg(objectArrayIndex);
                switch (i)
                {
                    case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                    default: il.Emit(OpCodes.Ldc_I4, i); break;
                }
                il.Emit(OpCodes.Ldelem_Ref);
                Type paramType = parameters[i].ParameterType;
                il.Emit(paramType.IsValueType
                    ? OpCodes.Unbox_Any
                    : OpCodes.Castclass, paramType);
            }
            return il;
        }

        public static ILGenerator LoadArg(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0: il.Emit(OpCodes.Ldarg_0); break;
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default: il.Emit(OpCodes.Ldarg_S, index); break;
            }
            return il;
        }

        public static ILGenerator LoadField(this ILGenerator il, FieldInfo field)
        {
            il.Emit(OpCodes.Ldfld, field);
            return il;
        }

        public static ILGenerator SetField(this ILGenerator il, FieldInfo field)
        {
            il.Emit(OpCodes.Stfld, field);
            return il;
        }

        public static ILGenerator CallMethod(this ILGenerator il, MethodInfo method)
        {
            OpCode call = method.IsVirtual
                ? OpCodes.Callvirt
                : OpCodes.Call;
            il.Emit(call, method);
            return il;
        }

        public static ILGenerator BoxIfNeeded(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Box, type);
            return il;
        }

        public static ILGenerator UnboxIfNeeded(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            return il;
        }

        public static void Return(this ILGenerator il)
        {
            il.Emit(OpCodes.Ret);
        }
    }
}