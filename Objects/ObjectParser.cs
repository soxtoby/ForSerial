using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace json.Objects
{
    public class ObjectParser : Parser
    {
        private readonly ParseValueFactory valueFactory;
        private readonly Options options;
        private object currentObject;
        private readonly Dictionary<object, ParseObject> objectReferences = new Dictionary<object, ParseObject>(new ReferenceEqualityComparer<object>());

        private ObjectParser(ParseValueFactory valueFactory, Options options)
        {
            this.valueFactory = valueFactory;
            this.options = options;
        }

        public static ParseValue Parse(object obj, ParseValueFactory valueFactory, Options options = Options.Default)
        {
            ObjectParser parser = new ObjectParser(valueFactory, options);

            return parser.ParseValue(obj);
        }

        public ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            return Parse(currentObject, subParseValueFactory).AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return valueFactory.CreateNull();

            switch (input.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    if (TypeDefinition.GetTypeDefinition(input.GetType()).IsJsonCompatibleDictionary)
                        return ParseDictionary((IDictionary)input);
                    IEnumerable enumerable = input as IEnumerable;
                    if (enumerable != null)
                        return ParseArray(enumerable);
                    return ParseObject(input);

                case TypeCodeType.Boolean:
                    return valueFactory.CreateBoolean((bool)input);

                case TypeCodeType.String:
                    return valueFactory.CreateString((string)input);

                case TypeCodeType.Number:
                    return valueFactory.CreateNumber(Convert.ToDouble(input));

                default:
                    throw new UnknownTypeCode(input);
            }
        }

        private ParseObject ParseDictionary(IDictionary dictionary)
        {
            ParseObject obj = valueFactory.CreateObject();

            obj.SetType(GetTypeIdentifier(dictionary.GetType()), this);

            foreach (object key in dictionary.Keys)
            {
                ParseValue value = ParseValue(dictionary[key]);

                // Convert.ToString is in case the keys are numbers, which are represented
                // as strings when used as keys, but can be indexed with numbers in JavaScript
                value.AddToObject(obj, Convert.ToString(key, CultureInfo.InvariantCulture));
            }

            return obj;
        }

        private ParseObject ParseObject(object obj)
        {
            ParseObject previouslyParsedObject = objectReferences.Get(obj);
            return previouslyParsedObject == null
                ? ParseNewObject(obj)
                : ReferenceObject(previouslyParsedObject);
        }

        private ParseObject ParseNewObject(object obj)
        {
            ParseObject output = objectReferences[obj] = valueFactory.CreateObject();

            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(obj.GetType());

            currentObject = obj;

            output.SetType(GetTypeIdentifier(typeDef.Type), this);

            IEnumerable<PropertyDefinition> propertiesToSerialize = SerializeOneWayTypes
                                                                        ? typeDef.Properties.Values
                                                                        : typeDef.SerializableProperties;

            foreach (PropertyDefinition property in propertiesToSerialize)
            {
                ParseValue value = ParseValue(property.GetFrom(obj));
                value.AddToObject(output, property.Name);
            }

            return output;
        }

        private static string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private bool SerializeOneWayTypes
        {
            get { return (options & Options.SerializeOneWayTypes) != 0; }
        }

        private ParseObject ReferenceObject(ParseObject parseObject)
        {
            return valueFactory.CreateReference(parseObject);
        }

        private ParseArray ParseArray(IEnumerable input)
        {
            ParseArray array = valueFactory.CreateArray();
            foreach (object item in input)
            {
                ParseValue value = ParseValue(item);
                value.AddToArray(array);
            }
            return array;
        }

        public class UnknownTypeCode : Exception
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
            SerializeOneWayTypes = 1,
        }
    }
}

