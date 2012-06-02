namespace ForSerial.Objects
{
    public class IgnoreAttribute : PropertyDefinitionAttribute
    {
        private readonly string[] ignoreScenarios;
        private readonly bool ignoreAll;

        public IgnoreAttribute(params string[] ignoreScenarios)
        {
            this.ignoreScenarios = ignoreScenarios;
            ignoreAll = ignoreScenarios.None();
        }

        public override bool CanGet
        {
            get
            {
                if (!ignoreAll)
                {
                    string currentScenario = SerializationScenario.Current;
                    for (int i = 0; i < ignoreScenarios.Length; i++)
                        if (ignoreScenarios[i] == currentScenario)
                            return false;

                    return base.CanGet;
                }
                return false;
            }
        }

        public override bool CanSet
        {
            get
            {
                if (!ignoreAll)
                {
                    string currentScenario = SerializationScenario.Current;
                    for (int i = 0; i < ignoreScenarios.Length; i++)
                        if (ignoreScenarios[i] == currentScenario)
                            return false;

                    return base.CanSet;
                }
                return false;
            }
        }

        public override ObjectContainer CreateStructure()
        {
            if (ignoreAll)
                return NullObjectStructure.Instance;

            string currentScenario = SerializationScenario.Current;
            for (int i = 0; i < ignoreScenarios.Length; i++)
                if (currentScenario == ignoreScenarios[i])
                    return NullObjectStructure.Instance;

            return base.CreateStructure();
        }

        public override ObjectContainer CreateStructure(string typeIdentifier)
        {
            if (ignoreAll)
                return NullObjectStructure.Instance;

            string currentScenario = SerializationScenario.Current;
            for (int i = 0; i < ignoreScenarios.Length; i++)
                if (currentScenario == ignoreScenarios[i])
                    return NullObjectStructure.Instance;

            return base.CreateStructure(typeIdentifier);
        }

        public override ObjectContainer CreateSequence()
        {
            if (ignoreAll)
                return NullObjectSequence.Instance;

            string currentScenario = SerializationScenario.Current;
            for (int i = 0; i < ignoreScenarios.Length; i++)
                if (currentScenario == ignoreScenarios[i])
                    return NullObjectSequence.Instance;

            return base.CreateSequence();
        }

        public override bool CanCreateValue(object value)
        {
            if (!ignoreAll)
            {
                string currentScenario = SerializationScenario.Current;
                for (int i = 0; i < ignoreScenarios.Length; i++)
                    if (ignoreScenarios[i] == currentScenario)
                        return false;

                return base.CanCreateValue(value);
            }
            return false;
        }

        public override ObjectOutput CreateValue(object value)
        {
            if (ignoreAll)
                return NullObjectValue.Instance;

            string currentScenario = SerializationScenario.Current;
            for (int i = 0; i < ignoreScenarios.Length; i++)
                if (currentScenario == ignoreScenarios[i])
                    return NullObjectValue.Instance;

            return base.CreateValue(value);
        }

        public override void Read(object value, ObjectReader reader, Writer writer)
        {
            if (!ignoreAll)
            {
                string currentScenario = SerializationScenario.Current;
                for (int i = 0; i < ignoreScenarios.Length; i++)
                    if (currentScenario == ignoreScenarios[i])
                        return;

                base.Read(value, reader, writer);
            }
        }

        public override bool MatchesPropertyFilter(MemberAccessibility requiredAccessibility, MemberType requiredType)
        {
            if (!ignoreAll)
            {
                string currentScenario = SerializationScenario.Current;
                for (int i = 0; i < ignoreScenarios.Length; i++)
                    if (ignoreScenarios[i] == currentScenario)
                        return false;

                return base.MatchesPropertyFilter(requiredAccessibility, requiredType);
            }
            return false;
        }
    }

    public class CopyIgnoreAttribute : IgnoreAttribute
    {
        public CopyIgnoreAttribute() : base(SerializationScenario.ObjectCopy) { }
    }

    public class JsonIgnoreAttribute : IgnoreAttribute
    {
        public JsonIgnoreAttribute() : base(SerializationScenario.SerializeToJson, SerializationScenario.DeserializeJson) { }
    }
}