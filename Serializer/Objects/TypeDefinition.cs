using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    public abstract class TypeDefinition
    {
        private static readonly HashSet<Type> IgnoreAttributes = new HashSet<Type>();

        private readonly TypeCode typeCode;
        private readonly List<PreBuildInfo> preBuildMethods = new List<PreBuildInfo>();

        public Type Type { get; private set; }
        public IDictionary<string, PropertyDefinition> Properties { get; private set; }

        public List<ConstructorDefinition> Constructors { get; private set; }

        protected TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
            Constructors = new List<ConstructorDefinition>();
            typeCode = Type.GetTypeCode(type);
        }

        private bool? isSerializable;
        public virtual bool IsSerializable
        {
            get
            {
                return isSerializable ?? (bool)(isSerializable =
                    !Type.IsAbstract
                    && (Type.IsSerializable || HasDefaultConstructor));
            }
        }

        private bool? isDeserializable;
        public virtual bool IsDeserializable
        {
            get
            {
                return isDeserializable ?? (bool)(isDeserializable =
                    !Type.IsAbstract
                    && HasDefaultConstructor);
            }
        }

        private bool HasDefaultConstructor
        {
            get { return Type.GetConstructor(new Type[] { }) != null; }
        }

        internal void Populate()
        {
            PopulateConstructors();
            PopulateProperties();
            PopulatePreBuildMethods();
        }

        private void PopulateConstructors()
        {
            Constructors.AddRange(Type.GetConstructors().Select(BuildConstructorDefinition));
        }

        private static ConstructorDefinition BuildConstructorDefinition(ConstructorInfo constructorInfo)
        {
            ObjectInterfaceProvider interfaceProvider = new ReflectionInterfaceProvider();
            IEnumerable<ParameterDefinition> parameters = constructorInfo
                .GetParameters()
                .Select(p => new ParameterDefinition(p.Name, p.ParameterType));
            Constructor constructorMethod = interfaceProvider.GetConstructor(constructorInfo);
            return new ConstructorDefinition(constructorMethod, parameters);
        }

        private void PopulateProperties()
        {
            PropertyDefinitionBuilder propBuilder = new PropertyDefinitionBuilder(new ReflectionInterfaceProvider());
            IEnumerable<PropertyDefinition> properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(NotMarkedWithIgnoreAttribute)
                .Select(propBuilder.Build);

            foreach (PropertyDefinition property in properties)
            {
                Properties[property.Name] = property;
            }
        }

        private static bool NotMarkedWithIgnoreAttribute(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(true);
            lock (IgnoreAttributes)
            {
                return attributes.None(a => IgnoreAttributes.Contains(a.GetType()));
            }
        }

        private void PopulatePreBuildMethods()
        {
            preBuildMethods.AddRange(Type.GetMethods()
                .SelectMany(m => m.GetCustomAttributes(typeof(PreBuildAttribute), true)
                    .Select(a => ValidateAndCreatePreBuildInfo((PreBuildAttribute)a, m))));
        }

        private static PreBuildInfo ValidateAndCreatePreBuildInfo(PreBuildAttribute preBuildAttribute, MethodInfo method)
        {
            ObjectInterfaceProvider interfaceProvider = new ReflectionInterfaceProvider();
            preBuildAttribute.AssertValidMethod(method);
            return new PreBuildInfo(preBuildAttribute, interfaceProvider.GetMethod(method));
        }

        /// <summary>
        /// Ensures that if the value is a number, it has the correct type.
        /// </summary>
        public object ConvertToCorrectType(object obj)
        {
            return typeCode.GetTypeCodeType() == TypeCodeType.Number
                ? System.Convert.ChangeType(obj, typeCode)
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

        internal PreBuildInfo GetPreBuildInfo(Reader reader)
        {
            return reader == null ? null : preBuildMethods.FirstOrDefault(pb => pb.ReaderMatches(reader));
        }

        public abstract Output ReadObject(object input, ReaderWriter valueFactory);

        protected static bool ValueIsSerializable(object value)
        {
            if (value == null) return true;

            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(value.GetType());
            return typeDef.IsSerializable;
        }

        public virtual Output CreateValue(object value)
        {
            if (value == null) return TypedNull.Value;
            throw new NotAValue(Type);
        }

        public virtual TypedObject CreateStructure()
        {
            throw new NotAnObject(Type);
        }

        public virtual TypedSequence CreateSequence()
        {
            throw new NotAnArray(Type);
        }

        public Writer GetWriterForProperty(string name)
        {
            PropertyDefinition property = Properties.Get(name);
            if (property != null)
                return property.Writer;
            return NullTypedWriter.Instance;
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

