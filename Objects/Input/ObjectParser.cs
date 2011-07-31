using System;
using System.Collections.Generic;

namespace json.Objects
{
    public partial class ObjectParser : Parser
    {
        private readonly Options options;
        private object currentObject;
        private readonly Dictionary<object, ParseObject> objectReferences = new Dictionary<object, ParseObject>(new ReferenceEqualityComparer<object>());

        private ObjectParser(ParseValueFactory valueFactory, Options options)
            : base(valueFactory)
        {
            this.options = options;
        }

        public static ParseValue Parse(object obj, ParseValueFactory valueFactory, Options options = Options.Default)
        {
            ObjectParser parser = new ObjectParser(valueFactory, options);

            return parser.ParseValue(obj);
        }

        public override ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            return Parse(currentObject, subParseValueFactory, options).AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return ValueFactory.CreateValue(null);

            Type inputType = input.GetType();

            ParseValue output = null;

            if (IsValueType(inputType))
                output = ValueFactory.CreateValue(input);

            if (output == null)
            {
                ParseObject previouslyParsedObject = objectReferences.Get(input);
                output = previouslyParsedObject == null
                    ? ParseObject(input)
                    : ReferenceObject(previouslyParsedObject);
            }

            return output;
        }

        private static bool IsValueType(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        private ParseValue ParseObject(object input)
        {
            currentObject = input;
            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(input.GetType());
            return typeDef.ParseObject(input, new ObjectParserValueFactory(this));
        }

        private static string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private ParseObject ReferenceObject(ParseObject parseObject)
        {
            return ValueFactory.CreateReference(parseObject);
        }

        internal class UnknownTypeCode : Exception
        {
            public UnknownTypeCode(object obj)
                : base("Type {0} has unknown TypeCode.".FormatWith(obj.GetType().FullName))
            { }
        }

        [Flags]
        public enum Options
        {
            Default = 0,

            /// <summary>
            /// Allows serialization of types that cannot be deserialized.
            /// </summary>
            SerializeAllTypes = 1,
        }
    }
}

