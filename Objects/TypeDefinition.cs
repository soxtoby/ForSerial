using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    public class TypeDefinition
    {
        public Type Type { get; private set; }
        public IDictionary<string, PropertyDefinition> Properties { get; private set; }
        private readonly List<PreBuildInfo> preBuildMethods = new List<PreBuildInfo>();
        public bool IsSerializable { get; private set; }
        public bool IsDeserializable { get; private set; }
        public bool IsJsonCompatibleDictionary { get; private set; }
        private readonly TypeCode typeCode;
        private static readonly HashSet<Type> IgnoreAttributes = new HashSet<Type>();

        private TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
            IsSerializable = DetermineIfSerializable();
            IsDeserializable = DetermineIfDeserializable();
            IsJsonCompatibleDictionary = DetermineIfJsonCompatibleDictionary();
            typeCode = Type.GetTypeCode(type);
        }

        private void Populate()
        {
            PopulateProperties();
            PopulatePreBuildMethods();
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

        private bool DetermineIfJsonCompatibleDictionary()
        {
            Type keyType = Type.GetGenericInterfaceType(typeof(IDictionary<,>));
            TypeCodeType typeCodeType = keyType.GetTypeCodeType();

            return typeCodeType == TypeCodeType.String
                   || typeCodeType == TypeCodeType.Number;
        }

        // FIXME Make this thread-safe - use a ConcurrentDictionary. This version of Mono doesn't appear to have it :(
        private static readonly Dictionary<string, TypeDefinition> KnownTypes = new Dictionary<string, TypeDefinition>();

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if (type == null) return null;

            if (!KnownTypes.ContainsKey(type.AssemblyQualifiedName))
            {
                TypeDefinition typeDef = KnownTypes[type.AssemblyQualifiedName] = new TypeDefinition(type);
                typeDef.Populate();
            }

            return KnownTypes[type.AssemblyQualifiedName];
        }

        public static TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            return KnownTypes.ContainsKey(assemblyQualifiedName)
                ? KnownTypes[assemblyQualifiedName]
                : GetTypeDefinition(Type.GetType(assemblyQualifiedName));
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

        internal PreBuildInfo GetPreBuildInfo(Parser parser)
        {
            return parser == null ? null : preBuildMethods.FirstOrDefault(pb => pb.ParserMatches(parser));
        }

        internal class PreBuildInfo
        {
            private readonly PreBuildAttribute attribute;
            private readonly MethodInfo method;

            public PreBuildInfo(PreBuildAttribute attribute, MethodInfo method)
            {
                this.attribute = attribute;
                this.method = method;
            }

            public void PreBuild(object target, Parser parser, ParseValueFactory objectPopulator)
            {
                ParseValueFactory contextBuilder = attribute.GetBuilder();
                ParseObject parsedContext = parser.ParseSubObject(contextBuilder);
                object context = attribute.GetContextValue(parsedContext);

                object preBuildResult = method.Invoke(target, new[] { context });

                attribute.ParsePreBuildResult(preBuildResult, objectPopulator);
            }

            public bool ParserMatches(Parser parser)
            {
                return attribute.ParserMatches(parser);
            }
        }
    }
}

