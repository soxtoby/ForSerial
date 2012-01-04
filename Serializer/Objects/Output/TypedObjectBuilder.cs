using System;

namespace json.Objects
{
    public partial class TypedObjectBuilder : ParseValueFactory
    {
        private Type baseType;

        private TypedObjectBuilder() { }

        public TypedObjectBuilder(Type baseType)
        {
            this.baseType = baseType;
        }

        private static TypedObjectBuilder instance;
        public static TypedObjectBuilder GenericInstance
        {
            get { return instance ?? (instance = new TypedObjectBuilder()); }
        }

        public static T GetResult<T>(ParseValue value)
        {
            TypedObjectArray array = value as TypedObjectArray;
            if (array != null)
                return (T)array.GetTypedValue();

            TypedObjectObject obj = value.AsObject() as TypedObjectObject;
            if (obj == null)
                throw new InvalidResultObject();

            return (T)obj.Object;
        }

        public ParseValue CreateValue(object value)
        {
            if (value == null)
                return TypedObjectNull.Value;

            switch (value.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    return new TypedObjectObject(value);

                case TypeCodeType.Boolean:
                    return (bool)value
                        ? TypedObjectBoolean.True
                        : TypedObjectBoolean.False;

                case TypeCodeType.String:
                    return new TypedObjectString((string)value);

                case TypeCodeType.Number:
                    return new TypedObjectNumber(Convert.ToDouble(value));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual ParseObject CreateObject()
        {
            if (baseType == null)
                return new TypedObjectObject();

            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(baseType);
            if (!typeDef.IsDeserializable)
                return new TypedObjectObject();

            TypedObjectObject obj = new TypedObjectObject(typeDef);
            baseType = null;    // Only needed for first object
            return obj;
        }

        public ParseArray CreateArray()
        {
            if (baseType == null)
                throw new UnknownRootArrayType();

            TypedObjectArray array = CurrentTypeHandler.GetTypeDefinition(baseType).CreateArray();
            baseType = null;    // Only needed for first object
            return array;
        }

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return parseObject;
        }

        internal class TypedObjectSubBuilder : TypedObjectBuilder
        {
            private readonly object baseObject;
            private bool isBase = true;

            public TypedObjectSubBuilder(object baseObject)
            {
                this.baseObject = baseObject;
            }

            public override ParseObject CreateObject()
            {
                if (!isBase)
                    return base.CreateObject();

                isBase = false;
                return new TypedObjectObject(baseObject);
            }
        }

        internal class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type TypedObjectArray.") { }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a TypedObjectBuilder.") { }
        }

        internal class UnknownRootArrayType : Exception
        {
            public UnknownRootArrayType() : base("Can't create array without a known type.") { }
        }
    }
}

