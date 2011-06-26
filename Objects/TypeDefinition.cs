using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    internal class TypeDefinition
    {
        public Type Type { get; private set; }
        public IDictionary<string, PropertyDefinition> Properties { get; private set; }
        private readonly List<PreBuildInfo> preBuildMethods = new List<PreBuildInfo>();
        public bool IsSerializable { get; private set; }

        private TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
            PopulateProperties();
            PopulatePreBuildMethods();
            IsSerializable = DetermineIfSerializable();
        }

        private bool DetermineIfSerializable()
        {
            return Type.IsSerializable
                || Type.GetConstructor(new Type[] { }) != null;
        }

        // FIXME Make this thread-safe - use a ConcurrentDictionary. This version of Mono doesn't appear to have it :(
        private static readonly Dictionary<string, TypeDefinition> knownTypes = new Dictionary<string, TypeDefinition>();

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if (!knownTypes.ContainsKey(type.AssemblyQualifiedName))
                knownTypes[type.AssemblyQualifiedName] = new TypeDefinition(type);

            return knownTypes[type.AssemblyQualifiedName];
        }

        public static TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            if (!knownTypes.ContainsKey(assemblyQualifiedName))
                knownTypes[assemblyQualifiedName] = new TypeDefinition(Type.GetType(assemblyQualifiedName));
            return knownTypes[assemblyQualifiedName];
        }

        public IEnumerable<PropertyDefinition> SerializableProperties
        {
            get { return Properties.Values.Where(p => p.IsSerializable); }
        }

        private void PopulateProperties()
        {
            IEnumerable<PropertyDefinition> properties =
                Type.GetProperties().Select(p => new PropertyDefinition(p));

            foreach (PropertyDefinition property in properties)
            {
                Properties[property.Name] = property;
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

        public bool PreBuild(object target, Parser parser, Func<ParseValueFactory> getObjectPopulator)
        {
            PreBuildInfo preBuildInfo = GetPreBuildInfo(parser);

            if (preBuildInfo == null)
                return false;

            preBuildInfo.PreBuild(target, parser, getObjectPopulator());

            return true;
        }

        private PreBuildInfo GetPreBuildInfo(Parser parser)
        {
            return parser == null ? null : preBuildMethods.FirstOrDefault(pb => pb.ParserMatches(parser));
        }

        private class PreBuildInfo
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

