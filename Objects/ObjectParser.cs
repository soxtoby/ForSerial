using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace json.Objects
{
    public class ObjectParser : Parser
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
            return Parse(currentObject, subParseValueFactory).AsObject();
        }

        private ParseValue ParseValue(object input)
        {
            if (input == null)
                return ValueFactory.CreateNull();

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
                    return ValueFactory.CreateBoolean((bool)input);

                case TypeCodeType.String:
                    return ValueFactory.CreateString((string)input);

                case TypeCodeType.Number:
                    return ValueFactory.CreateNumber(Convert.ToDouble(input));

                default:
                    throw new UnknownTypeCode(input);
            }
        }

        private ParseObject ParseDictionary(IDictionary dictionary)
        {
            ParseObject obj = ValueFactory.CreateObject();

            obj.SetType(GetTypeIdentifier(dictionary.GetType()), this);

            foreach (object key in dictionary.Keys)
            {
                // Convert.ToString is in case the keys are numbers, which are represented
                // as strings when used as keys, but can be indexed with numbers in JavaScript
                string name = Convert.ToString(key, CultureInfo.InvariantCulture);
                object value = dictionary[key];

                UsingObjectPropertyContext(obj, name, () =>
                {
                    ParseValue parseValue = ParseValue(value);
                    parseValue.AddToObject(obj, name);
                });
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
            ParseObject output = objectReferences[obj] = ValueFactory.CreateObject();

            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(obj.GetType());

            currentObject = obj;

            output.SetType(GetTypeIdentifier(typeDef.Type), this);

            IEnumerable<PropertyDefinition> propertiesToSerialize = SerializeOneWayTypes
                                                                        ? typeDef.Properties.Values
                                                                        : typeDef.SerializableProperties;

            foreach (PropertyDefinition property in propertiesToSerialize)
            {
                PropertyDefinition prop = property;
                UsingObjectPropertyContext(output, property.Name, () =>
                {
                    ParseValue value = ParseValue(prop.GetFrom(obj));
                    value.AddToObject(output, prop.Name);
                });
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
            return ValueFactory.CreateReference(parseObject);
        }

        private ParseArray ParseArray(IEnumerable input)
        {
            ParseArray array = ValueFactory.CreateArray();
            UsingArrayContext(array, () =>
            {
                foreach (object item in input)
                {
                    ParseValue value = ParseValue(item);
                    value.AddToArray(array);
                }
            });
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

