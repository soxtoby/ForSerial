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

        protected static readonly ObjectInterfaceProvider ObjectInterfaceProvider = new DynamicMethodProvider();

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
            IEnumerable<ParameterDefinition> parameters = constructorInfo
                .GetParameters()
                .Select(p => new ParameterDefinition(p.Name, p.ParameterType));
            ConstructorMethod constructorMethod = ObjectInterfaceProvider.GetConstructor(constructorInfo);
            return new ConstructorDefinition(constructorMethod, parameters);
        }

        private void PopulateProperties()
        {
            if (Type.IsInterface)
                return;

            PropertyDefinitionBuilder propBuilder = new PropertyDefinitionBuilder(ObjectInterfaceProvider);
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

        public abstract void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier);

        protected static bool ValueIsSerializable(object value)
        {
            if (value == null) return true;

            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(value.GetType());
            return typeDef.IsSerializable;
        }

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

        // TODO this property stuff should be on StructureDefinition or something - SequenceDefinitions don't have properties

        public virtual ObjectContainer CreateStructureForProperty(string name)
        {
            PropertyDefinition property = Properties.Get(name);
            return property != null
                ? property.CreateStructure()
                : NullObjectStructure.Instance;
        }

        public ObjectContainer CreateStructureForProperty(string name, string typeIdentifier)
        {
            PropertyDefinition property = Properties.Get(name);
            return property != null
                ? property.CreateStructure(typeIdentifier)
                : NullObjectStructure.Instance;
        }

        public virtual ObjectContainer CreateSequenceForProperty(string name)
        {
            PropertyDefinition property = Properties.Get(name);
            return property != null
                ? property.CreateSequence()
                : NullObjectSequence.Instance;
        }

        public bool CanCreateValueForProperty(string name, object value)
        {
            PropertyDefinition property = Properties.Get(name);
            return property != null && property.TypeDef.CanCreateValue(value);
        }

        public virtual ObjectValue CreateValueForProperty(string name, object value)
        {
            PropertyDefinition property = Properties.Get(name);
            return property != null
                ? property.CreateValue(value)
                : NullObjectValue.Instance;
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

