using ForSerial.Objects;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class SerializeAttributeAllScenariosTests : AttributeTests<SerializeAttribute>
    {
        [Test]
        public void MatchesPropertyFilter_Matches()
        {
            TestMatchesPropertyFilterOverride("foo", false, true);
        }

        protected override SerializeAttribute CreateAttribute()
        {
            return new SerializeAttribute();
        }
    }

    [TestFixture]
    public class SerializeAttributeSpecificScenarioTests : AttributeTests<SerializeAttribute>
    {
        private const string SerializeForScenario = "foo";

        [Test]
        public void MatchesPropertyFilter_WrongScenario_PassesThrough()
        {
            TestMatchesPropertyFilterOverride(null, false, false);
        }

        [Test]
        public void MatchesPropertyFilter_SerializeScenario_Matches()
        {
            TestMatchesPropertyFilterOverride(SerializeForScenario, false, true);
        }

        protected override SerializeAttribute CreateAttribute()
        {
            return new SerializeAttribute(SerializeForScenario);
        }
    }

    [TestFixture]
    public class ForceCopyAttributeTests : AttributeTests<ForceCopyAttribute>
    {
        [Test]
        public void MatchesPropertyFilter_WrongScenario_PassesThrough()
        {
            TestMatchesPropertyFilterOverride(null, false, false);
        }

        [Test]
        public void MatchesPropertyFilter_ObjectCopy_Matches()
        {
            TestMatchesPropertyFilterOverride(SerializationScenario.ObjectCopy, false, true);
        }

        protected override ForceCopyAttribute CreateAttribute()
        {
            return new ForceCopyAttribute();
        }
    }

    [TestFixture]
    public class JsonSerializeAttributeTests : AttributeTests<JsonSerializeAttribute>
    {
        [Test]
        public void MatchesPropertyFilter_WrongScenario_PassesThrough()
        {
            TestMatchesPropertyFilterOverride(null, false, false);
        }

        [Test]
        public void MatchesPropertyFilter_SerializeToJson_Matches()
        {
            TestMatchesPropertyFilterOverride(SerializationScenario.SerializeToJson, false, true);
        }

        protected override JsonSerializeAttribute CreateAttribute()
        {
            return new JsonSerializeAttribute();
        }
    }
}