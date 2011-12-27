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

        protected TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
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
            PopulateProperties();
            PopulatePreBuildMethods();
        }

        private void PopulateProperties()
        {
            IEnumerable<PropertyDefinition> properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(NotMarkedWithIgnoreAttribute)
                .Select(p => new PropertyDefinition(p));

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
            return new PreBuildInfo(preBuildAttribute, method);
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

        internal PreBuildInfo GetPreBuildInfo(Parser parser)
        {
            return parser == null ? null : preBuildMethods.FirstOrDefault(pb => pb.ParserMatches(parser));
        }

        public abstract ParseValue ParseObject(object input, ParserValueFactory valueFactory);

        protected static bool ValueIsSerializable(object value)
        {
            if (value == null) return true;

            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(value.GetType());
            return typeDef.IsSerializable;
        }

        public virtual ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            if (value == null) return TypedObjectNull.Value;
            throw new NotAValue(Type);
        }

        public virtual TypedObjectParseObject CreateObject()
        {
            throw new NotAnObject(Type);
        }

        public virtual TypedObjectArray CreateArray()
        {
            throw new NotAnArray(Type);
        }

        private class NotAValue : Exception
        {
            public NotAValue(Type type) : base("Cannot create value for type {0}.".FormatWith(type.FullName)) { }
        }

        private class NotAnObject : Exception
        {
            public NotAnObject(Type type) : base("Cannot create object for type {0}".FormatWith(type.FullName)) { }
        }

        protected class NotAnArray : Exception
        {
            public NotAnArray(Type type) : base("Cannot create array for type {0}".FormatWith(type.FullName)) { }
        }
    }
}

