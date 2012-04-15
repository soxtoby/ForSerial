
namespace ForSerial.Objects
{
    public class IgnoreAttribute : PropertyDefinitionAttribute
    {
        private readonly string ignoreScenario;
        private readonly bool ignoreAll;

        public IgnoreAttribute(string ignoreScenario = null)
        {
            this.ignoreScenario = ignoreScenario;
            ignoreAll = ignoreScenario == null;
        }

        public override bool CanGet
        {
            get
            {
                return !ignoreAll
                    && SerializationScenario.Current != ignoreScenario
                    && base.CanGet;
            }
        }

        public override bool CanSet
        {
            get
            {
                return !ignoreAll
                    && SerializationScenario.Current != ignoreScenario
                    && base.CanSet;
            }
        }

        public override ObjectContainer CreateStructure()
        {
            return ignoreAll || SerializationScenario.Current == ignoreScenario
                ? NullObjectStructure.Instance
                : base.CreateStructure();
        }

        public override ObjectContainer CreateStructure(string typeIdentifier)
        {
            return ignoreAll || SerializationScenario.Current == ignoreScenario
                ? NullObjectStructure.Instance
                : base.CreateStructure(typeIdentifier);
        }

        public override ObjectContainer CreateSequence()
        {
            return ignoreAll || SerializationScenario.Current == ignoreScenario
                ? NullObjectSequence.Instance
                : base.CreateSequence();
        }

        public override bool CanCreateValue(object value)
        {
            return !ignoreAll
                && SerializationScenario.Current != ignoreScenario
                && base.CanCreateValue(value);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return ignoreAll || SerializationScenario.Current == ignoreScenario
                ? NullObjectValue.Instance
                : base.CreateValue(value);
        }

        public override void Read(object value, ObjectReader reader, Writer writer)
        {
            if (!ignoreAll && SerializationScenario.Current != ignoreScenario)
                base.Read(value, reader, writer);
        }

        public override bool MatchesPropertyFilter(PropertyFilter filter)
        {
            return !ignoreAll
                && SerializationScenario.Current != ignoreScenario
                && base.MatchesPropertyFilter(filter);
        }
    }

    public class CopyIgnoreAttribute : IgnoreAttribute
    {
        public CopyIgnoreAttribute() : base(SerializationScenario.ObjectCopy) { }
    }

    public class JsonIgnoreAttribute : IgnoreAttribute
    {
        public JsonIgnoreAttribute() : base(SerializationScenario.SerializeToJson) { }
    }
}