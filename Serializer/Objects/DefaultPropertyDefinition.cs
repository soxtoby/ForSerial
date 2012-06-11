using System;

namespace ForSerial.Objects
{
    internal class DefaultPropertyDefinition : PropertyDefinition
    {
        public DefaultPropertyDefinition(TypeDefinition typeDef,
                                         string name,
                                         GetMethod getter,
                                         SetMethod setter,
                                         string declaringTypeName,
                                         MemberAccessibility memberAccessibility,
                                         MemberType memberType)
        {
            if (typeDef == null) throw new ArgumentNullException("typeDef");
            if (name == null) throw new ArgumentNullException("name");
            if (declaringTypeName == null) throw new ArgumentNullException("declaringTypeName");

            Name = name;
            FullName = declaringTypeName + "." + name;
            this.getter = getter;
            this.setter = setter;
            this.memberAccessibility = memberAccessibility;
            this.memberType = memberType;
            TypeDef = typeDef;
        }

        public string Name { get; private set; }
        public TypeDefinition TypeDef { get; private set; }

        public string FullName { get; private set; }
        private readonly GetMethod getter;
        private readonly SetMethod setter;
        private readonly MemberAccessibility memberAccessibility;
        private readonly MemberType memberType;

        public bool CanGet { get { return getter != null; } }
        public bool CanSet { get { return setter != null; } }

        public object GetFrom(object source)
        {
            return getter(source);
        }

        public void SetOn(object target, object value)
        {
            if (CanSet)
                setter(target, TypeDef.ConvertToCorrectType(value));
        }

        public ObjectContainer CreateStructure()
        {
            return TypeDef.CreateStructure();
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return TypeDef.CreateStructure(typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return TypeDef.CreateSequence();
        }

        public bool CanCreateValue(object value)
        {
            return TypeDef.CanCreateValue(value);
        }

        public ObjectOutput CreateValue(object value)
        {
            return TypeDef.CreateValue(value);
        }

        public void Read(object value, ObjectReader reader, Writer writer)
        {
            TypeDef.ReadObject(value, reader, writer, PartialOptions.Default);
        }

        public bool MatchesPropertyFilter(ObjectParsingOptions options)
        {
            return (options.MemberAccessibility & memberAccessibility) == options.MemberAccessibility
                && (options.MemberType & memberType) > 0;
        }
    }
}
