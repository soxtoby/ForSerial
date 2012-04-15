using ForSerial.Objects;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class JsonNameAttributeTests : AttributeTests<JsonNameAttribute>
    {
        private const string OverrideName = "override";
        private const string BaseName = "base";

        [Test]
        public void WrongScenario_NamePassesThrough()
        {
            TestNameOverride(SerializationScenario.ObjectCopy, BaseName, BaseName);
        }

        [Test]
        public void SerializeToJson_NameOverridden()
        {
            TestNameOverride(SerializationScenario.SerializeToJson, BaseName, OverrideName);
        }

        protected override JsonNameAttribute CreateAttribute()
        {
            return new JsonNameAttribute(OverrideName);
        }
    }
}