using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    public abstract class TypeDefinition
    {
        private bool isSealed;
        protected readonly TypeCode typeCode;
        protected static readonly HashSet<Type> IgnoreAttributes = new HashSet<Type>();
        private readonly List<PreBuildInfo> preBuildMethods = new List<PreBuildInfo>();

        public Type Type { get; private set; }
        public List<ConstructorDefinition> Constructors { get; private set; }

        protected static readonly ObjectInterfaceProvider ObjectInterfaceProvider = new DynamicMethodProvider();

        protected TypeDefinition(Type type)
        {
            Type = type;
            Constructors = new List<ConstructorDefinition>();
            typeCode = Type.GetTypeCode(type);
        }

        internal virtual void Populate()
        {
            isSealed = Type.IsSealed;
            PopulateConstructors();
            PopulatePreBuildMethods();
        }

        private void PopulateConstructors()
        {
            Constructors.AddRange(Type.GetConstructors().Select(BuildConstructorDefinition));
        }

        private static ConstructorDefinition BuildConstructorDefinition(ConstructorInfo constructorInfo)
        {
            IEnumerable<ParameterDefinition> parameters = constructorInfo
                .GetParameters()
                .Select(p => new ParameterDefinition(p.Name, p.ParameterType));
            ConstructorMethod constructorMethod = ObjectInterfaceProvider.GetConstructor(constructorInfo);
            return new ConstructorDefinition(constructorMethod, parameters);
        }

        private void PopulatePreBuildMethods()
        {
            preBuildMethods.AddRange(Type.GetMethods()
                .SelectMany(m => m.GetCustomAttributes(typeof(PreBuildAttribute), true)
                    .Select(a => ValidateAndCreatePreBuildInfo((PreBuildAttribute)a, m))));
        }

        private static PreBuildInfo ValidateAndCreatePreBuildInfo(PreBuildAttribute preBuildAttribute, MethodInfo method)
        {
            preBuildAttribute.AssertValidMethod(method);
            return new PreBuildInfo(preBuildAttribute, ObjectInterfaceProvider.GetStaticFunc(method));
        }

        /// <summary>
        /// Ensures that if the value is a number, it has the correct type.
        /// </summary>
        public object ConvertToCorrectType(object obj)
        {
            return typeCode.GetTypeCodeType() == TypeCodeType.Number
                ? Convert.ChangeType(obj, typeCode)
                : obj;
        }

        public static void IgnorePropertiesMarkedWithAttribute(Type attributeType)
        {
            if (!attributeType.CanBeCastTo(typeof(Attribute)))
                throw new ArgumentException("Type must derive from Attribute", "attributeType");

            lock (IgnoreAttributes)
            {
                IgnoreAttributes.Add(attributeType);
            }
        }

        internal PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return readerType == null ? null : preBuildMethods.FirstOrDefault(pb => pb.ReaderMatches(readerType));
        }

        public void ReadObject(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (input == null)
                writer.WriteNull();
            else if (isSealed || input.GetType() == Type)
                Read(input, reader, writer, requestTypeIdentification);
            else
            {
                TypeDefinition inputTypeDef = CurrentTypeHandler.GetTypeDefinition(input);
                inputTypeDef.Read(input, reader, writer, true);
            }
        }

        public abstract void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification);

        public virtual ObjectContainer CreateStructure()
        {
            throw new NotAnObject(Type);
        }

        public ObjectContainer CreateStructure(string requestedTypeIdentifier)
        {
            TypeDefinition requestedTypeDef = CurrentTypeHandler.GetTypeDefinition(requestedTypeIdentifier);
            return Type.IsAssignableFrom(requestedTypeDef.Type)
                ? requestedTypeDef.CreateStructure()
                : CreateStructure();
        }

        public virtual ObjectContainer CreateSequence()
        {
            throw new NotAnArray(Type);
        }

        public virtual bool CanCreateValue(object value)
        {
            if (value == null)
                return true;
            Type type = value.GetType();
            return type.IsValueType
                || type == typeof(string);
        }

        public virtual ObjectValue CreateValue(object value)
        {
            if (value == null) return new DefaultObjectValue(null);
            throw new NotAValue(Type);
        }

        private class NotAValue : Exception
        {
            public NotAValue(Type type) : base("Cannot create value for type {0}.".FormatWith(type.FullName)) { }
        }

        private class NotAnObject : Exception
        {
            public NotAnObject(Type type) : base("Cannot create object for type {0}".FormatWith(type.FullName)) { }
        }

        private class NotAnArray : Exception
        {
            public NotAnArray(Type type) : base("Cannot create sequence for type {0}".FormatWith(type.FullName)) { }
        }
    }
}
