namespace ForSerial.Objects
{
    public class SerializeAttribute : PropertyDefinitionAttribute
    {
        private readonly string[] scenarios;
        private readonly bool serializeForAll;

        public SerializeAttribute(params string[] scenarios)
        {
            this.scenarios = scenarios;
            serializeForAll = scenarios.None();
        }

        public override bool MatchesPropertyFilter(ObjectParsingOptions options)
        {
            if (serializeForAll)
                return true;

            string currentScenario = SerializationScenario.Current;
            for (int i = 0; i < scenarios.Length; i++)
                if (scenarios[i] == currentScenario)
                    return true;

            return base.MatchesPropertyFilter(options);
        }
    }

    public class ForceCopyAttribute : SerializeAttribute
    {
        public ForceCopyAttribute() : base(SerializationScenario.ObjectCopy) { }
    }

    public class JsonSerializeAttribute : SerializeAttribute
    {
        public JsonSerializeAttribute() : base(SerializationScenario.SerializeToJson) { }
    }
}