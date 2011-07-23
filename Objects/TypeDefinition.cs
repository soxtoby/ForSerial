using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    public abstract class TypeDefinition
    {
        private static readonly Dictionary<string, TypeDefinition> KnownTypes = new Dictionary<string, TypeDefinition>();
        private static readonly HashSet<Type> IgnoreAttributes = new HashSet<Type>();
        private static readonly List<Func<Type, TypeDefinition>> TypeDefinitionFactories = new List<Func<Type, TypeDefinition>>
            {
                DefaultTypeDefinition.CreateDefaultTypeDefinition,
                CollectionDefinition.CreateCollectionDefinition, // FIXME I'm not keen on this inter-dependency. Maybe move factories into another class.
                DictionaryDefinition.CreateDictionaryDefinition,
            };

        private readonly TypeCode typeCode;
        private readonly List<PreBuildInfo> preBuildMethods = new List<PreBuildInfo>();

        public Type Type { get; private set; }
        public IDictionary<string, PropertyDefinition> Properties { get; private set; }
        public bool IsSerializable { get; private set; }
        public bool IsDeserializable { get; private set; }

        protected TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
            IsSerializable = DetermineIfSerializable();
            IsDeserializable = DetermineIfDeserializable();
            typeCode = Type.GetTypeCode(type);
        }

        private bool DetermineIfSerializable()
        {
            return Type.IsSerializable
                || HasDefaultConstructor;
        }

        private bool DetermineIfDeserializable()
        {
            return Type.IsPrimitive
                || Type == typeof(string)   // Strings are objects
                || !Type.IsAbstract
                    && HasDefaultConstructor;
        }

        private bool HasDefaultConstructor
        {
            get { return Type.GetConstructor(new Type[] { }) != null; }
        }

        public static TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            return KnownTypes.ContainsKey(assemblyQualifiedName)
                ? KnownTypes[assemblyQualifiedName]
                : GetTypeDefinition(Type.GetType(assemblyQualifiedName));
        }

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if (type == null) return null;

            if (!KnownTypes.ContainsKey(type.AssemblyQualifiedName))
            {
                // Since this is where we automatically create a TypeDefinition, 
                // we need to register before we populate, in case the type contains itself.
                TypeDefinition typeDef = CreateTypeDefinition(type);
                RegisterTypeDefinition(typeDef);
                typeDef.Populate();
            }

            return KnownTypes[type.AssemblyQualifiedName];
        }

        private static void RegisterTypeDefinition(TypeDefinition typeDef)
        {
            KnownTypes[typeDef.Type.AssemblyQualifiedName] = typeDef;
        }

        private static TypeDefinition CreateTypeDefinition(Type type)
        {
            TypeDefinition typeDef = null;
            int i = TypeDefinitionFactories.Count;
            while (typeDef == null && i >= 0)
                typeDef = TypeDefinitionFactories[--i](type);
            return typeDef;
        }

        private void Populate()
        {
            PopulateProperties();
            PopulatePreBuildMethods();
        }

        private void PopulateProperties()
        {
            IEnumerable<PropertyDefinition> properties = Type.GetProperties()
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

        public virtual bool PropertyCanBeSerialized(PropertyDefinition property)
        {
            return property.CanGet && property.CanSet;
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

        public abstract ParseValue GetParseValue(ParseValueFactory valueFactory);

        public abstract void ParseObject(object input, ParseValue output, ParserValueFactory valueFactory);

        protected IEnumerable<KeyValuePair<string, object>> GetSerializableProperties(object obj, bool serializeAllTypes)
        {
            return Properties.Values
                .Where(p => serializeAllTypes || p.IsSerializable)
                .Select(p => new KeyValuePair<string, object>(p.Name, p.GetFrom(obj)))
                .Where(p => serializeAllTypes || ValueIsSerializable(p.Value));
        }

        private static bool ValueIsSerializable(object value)
        {
            if (value == null) return true;

            TypeDefinition typeDef = GetTypeDefinition(value.GetType());
            return typeDef.IsSerializable && typeDef.IsDeserializable;
        }
    }
}

