
using System;

namespace json.Objects
{
    public class PropertyDefinition
    {
        public PropertyDefinition(TypeDefinition typeDef,
                                    string name,
                                    GetMethod getter,
                                    SetMethod setter,
                                    bool forceTypeIdentifierSerialization,
                                    string declaringTypeName,
                                    bool hasPublicGetter,
                                    bool hasPublicSetter)
        {
            if (typeDef == null) throw new ArgumentNullException("typeDef");
            if (name == null) throw new ArgumentNullException("name");
            if (declaringTypeName == null) throw new ArgumentNullException("declaringTypeName");

            Name = name;
            FullName = declaringTypeName + "." + name;
            this.getter = getter;
            this.setter = setter;
            this.typeDef = typeDef;
            this.forceTypeIdentifierSerialization = forceTypeIdentifierSerialization;
            this.hasPublicGetter = hasPublicGetter;
            this.hasPublicSetter = hasPublicSetter;
        }

        public string Name { get; private set; }
        private readonly TypeDefinition typeDef;
        private readonly bool forceTypeIdentifierSerialization;
        private readonly bool hasPublicGetter;
        private readonly bool hasPublicSetter;

        internal readonly string FullName;
        private readonly GetMethod getter;
        private readonly SetMethod setter;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public object GetFrom(object source)
        {
            return getter(source);
        }

        public void SetOn(object target, object value)
        {
            if (CanSet)
                setter(target, typeDef.ConvertToCorrectType(value));
        }

        public ObjectContainer CreateStructure()
        {
            return typeDef.CreateStructure();
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return typeDef.CreateStructure(typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return typeDef.CreateSequence();
        }

        public bool CanCreateValue(object value)
        {
            return typeDef.CanCreateValue(value);
        }

        public ObjectValue CreateValue(object value)
        {
            return typeDef.CreateValue(value);
        }

        public void Read(object value, ObjectReader reader, Writer writer)
        {
            typeDef.ReadObject(value, reader, writer, forceTypeIdentifierSerialization);
        }

        public bool MatchesFilter(PropertyFilter propertyFilter)
        {
            switch (propertyFilter)
            {
                case PropertyFilter.PublicGet:
                    return hasPublicGetter;
                case PropertyFilter.PublicGetSet:
                    return hasPublicGetter && hasPublicSetter;
                default:
                    throw new ArgumentOutOfRangeException("propertyFilter");
            }
        }
    }
}
