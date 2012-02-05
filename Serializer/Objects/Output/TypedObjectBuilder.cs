using System;

namespace json.Objects
{
    public class TypedObjectBuilder : Writer
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

        public static T GetResult<T>(Output value)
        {
            TypedSequence array = value as TypedSequence;
            if (array != null)
                return (T)array.GetTypedValue();

            TypedObjectOutputStructure obj = value.AsStructure() as TypedObjectOutputStructure;
            if (obj == null)
                throw new InvalidResultObject();

            TypeDefinition outputTypeDef = CurrentTypeHandler.GetTypeDefinition(typeof (T));
            return (T)outputTypeDef.ConvertToCorrectType( obj.Object);
        }

        public Output CreateValue(object value)
        {
            if (value == null)
                return TypedNull.Value;

            if (baseType != null)
            {
                TypeDefinition baseTypeDef = CurrentTypeHandler.GetTypeDefinition(baseType);
                baseType = null;
                return baseTypeDef.CreateValue(value);
            }

            TypeDefinition valueTypeDef = CurrentTypeHandler.GetTypeDefinition(value.GetType());
            return valueTypeDef.CreateValue(value);
        }

        public virtual OutputStructure CreateStructure()
        {
            if (baseType == null)
                return new TypedObjectOutputStructure();

            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(baseType);
            TypedObjectOutputStructure obj = typeDef.IsDeserializable
                ? new TypedObjectOutputStructure(typeDef)
                : new TypedObjectOutputStructure();

            baseType = null;    // Only needed for first object
            return obj;
        }

        public SequenceOutput CreateSequence()
        {
            if (baseType == null)
                throw new UnknownRootArrayType();

            TypedSequence array = CurrentTypeHandler.GetTypeDefinition(baseType).CreateSequence();
            baseType = null;    // Only needed for first object
            return array;
        }

        public OutputStructure CreateReference(OutputStructure outputStructure)
        {
            return outputStructure;
        }

        internal class TypedObjectSubBuilder : TypedObjectBuilder
        {
            private readonly object baseObject;
            private bool isBase = true;

            public TypedObjectSubBuilder(object baseObject)
            {
                this.baseObject = baseObject;
            }

            public override OutputStructure CreateStructure()
            {
                if (!isBase)
                    return base.CreateStructure();

                isBase = false;
                return new TypedObjectOutputStructure(baseObject);
            }
        }

        internal class UnsupportedSequenceOutput : Exception
        {
            public UnsupportedSequenceOutput() : base("Can only add SequenceOutput of type TypedObjectArray.") { }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid OutputStructure type. Object must be constructed using a TypedObjectBuilder.") { }
        }

        internal class UnknownRootArrayType : Exception
        {
            public UnknownRootArrayType() : base("Can't create array without a known type.") { }
        }
    }
}

