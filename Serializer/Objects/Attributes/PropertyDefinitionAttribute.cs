using System;

namespace json.Objects
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyDefinitionAttribute : Attribute, PropertyDefinition
    {
        protected internal PropertyDefinition InnerDefinition;

        public virtual string Name
        {
            get { return InnerDefinition.Name; }
        }

        public TypeDefinition TypeDef { get { return InnerDefinition.TypeDef; } }

        public string FullName { get { return InnerDefinition.FullName; } }

        public virtual bool CanGet
        {
            get { return InnerDefinition.CanGet; }
        }

        public virtual bool CanSet
        {
            get { return InnerDefinition.CanSet; }
        }

        public virtual object GetFrom(object source)
        {
            return InnerDefinition.GetFrom(source);
        }

        public virtual void SetOn(object target, object value)
        {
            InnerDefinition.SetOn(target, value);
        }

        public virtual ObjectContainer CreateStructure()
        {
            return InnerDefinition.CreateStructure();
        }

        public virtual ObjectContainer CreateStructure(string typeIdentifier)
        {
            return InnerDefinition.CreateStructure(typeIdentifier);
        }

        public virtual ObjectContainer CreateSequence()
        {
            return InnerDefinition.CreateSequence();
        }

        public virtual bool CanCreateValue(object value)
        {
            return InnerDefinition.CanCreateValue(value);
        }

        public virtual ObjectValue CreateValue(object value)
        {
            return InnerDefinition.CreateValue(value);
        }

        public virtual void Read(object value, ObjectReader reader, Writer writer)
        {
            InnerDefinition.Read(value, reader, writer);
        }

        public virtual bool MatchesPropertyFilter(PropertyFilter filter)
        {
            return InnerDefinition.MatchesPropertyFilter(filter);
        }
    }
}