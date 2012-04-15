using System;

namespace ForSerial.Objects
{
    public class JsonNameAttribute : PropertyDefinitionAttribute
    {
        private readonly string name;

        public JsonNameAttribute(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.name = name;
        }

        public override string Name
        {
            get
            {
                return SerializationScenario.Current == SerializationScenario.SerializeToJson
                    || SerializationScenario.Current == SerializationScenario.DeserializeJson
                        ? name
                        : base.Name;
            }
        }
    }
}