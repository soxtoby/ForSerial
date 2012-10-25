using System;
using System.Collections.Generic;
using EasyAssertions;
using ForSerial.Objects;
using NUnit.Framework;
using System.Linq;

namespace ForSerial.Tests
{
    [TestFixture]
    public class TransformTests
    {
        private readonly string lf = Environment.NewLine;

        [Test]
        public void ToJson_SerializedToJsonString()
        {
            new { foo = 1 }.ToJson()
                .ShouldEndWith(@"""foo"":1}");  // Start will be type identifier
        }

        [Test]
        public void ToJson_PassesOptionsToObjectReader()
        {
            new { foo = 1 }.ToJson(new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe(@"{""foo"":1}");
        }

        [Test]
        public void ToJson_DefaultsToSerializeJsonScenario()
        {
            ScenarioCatcher result = new ScenarioCatcher();
            result.ToJson();
            result.Scenario.ShouldBe(SerializationScenario.SerializeToJson);
        }

        [Test]
        public void ToJson_GivenScenario_OverridesScenario()
        {
            ScenarioCatcher result = new ScenarioCatcher();
            result.ToJson(scenario: "foo");
            result.Scenario.ShouldBe("foo");
        }

        [Test]
        public void ToFormattedJson_SerializesToFormattedJsonString()
        {
            new[] { 1 }.ToFormattedJson()
                .ShouldBe("[" + lf + "  1" + lf + "]");
        }

        [Test]
        public void ToFormattedJson_PassesOptionsToObjectReader()
        {
            new { foo = 1 }.ToFormattedJson(new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.None })
                .ShouldBe("{" + lf + @"  ""foo"": 1" + lf + "}");
        }

        [Test]
        public void ToFormattedJson_PassesIndentationToPrettyPrinter()
        {
            new[] { 1 }.ToFormattedJson(indentation: "_")
                .ShouldBe("[" + lf + "_1" + lf + "]");
        }

        [Test]
        public void ToFormattedJson_DefaultsToSerializeJsonScenario()
        {
            ScenarioCatcher result = new ScenarioCatcher();
            result.ToFormattedJson();
            result.Scenario.ShouldBe(SerializationScenario.SerializeToJson);
        }

        [Test]
        public void ToFormattedJson_GivenScenario_OverridesScenario()
        {
            ScenarioCatcher result = new ScenarioCatcher();
            result.ToFormattedJson(scenario: "foo");
            result.Scenario.ShouldBe("foo");
        }

        [Test]
        public void ToFormattedJson_GivenString_FormatsJsonString()
        {
            "[1]".ToFormattedJson()
                .ShouldBe("[" + lf + "  1" + lf + "]");
        }

        [Test]
        public void ToFormattedJson_GivenString_PassesIndentationToPrettyPrinter()
        {
            "[1]".ToFormattedJson("_")
                .ShouldBe("[" + lf + "_1" + lf + "]");
        }

        [Test]
        public void ParseJson_ParsesToObject()
        {
            "[1]".ParseJson<List<int>>()
                .ShouldMatch(new[] { 1 });
        }

        [Test]
        public void ParseJson_DefaultsToDeserializeJsonScenario()
        {
            ScenarioCatcher result = @"{ ""Property"": 1 }".ParseJson<ScenarioCatcher>();
            result.Scenario.ShouldBe(SerializationScenario.DeserializeJson);
        }

        [Test]
        public void ParseJson_GivenScenario_OverridesScenario()
        {
            ScenarioCatcher result = @"{ ""Property"": 1 }".ParseJson<ScenarioCatcher>(scenario: "foo");
            result.Scenario.ShouldBe("foo");
        }

        [Test]
        public void CopyTo_CopiesToNewObject()
        {
            List<int> original = new List<int> { 1 };
            original.CopyTo<List<int>>()
                .ShouldNotReferTo(original)
                .And.ShouldMatch(new[] { 1 });
        }

        [Test]
        public void CopyTo_PassesOptionsToObjectReader()
        {
            new MixedAccessPropertiesClass(1) { PublicGetSet = 2 }
                .CopyTo<Dictionary<string, int>>(new ObjectParsingOptions { MemberAccessibility = MemberAccessibility.PublicGetSet })
                .ShouldMatch(new KeyValuePair<string, int>("PublicGetSet", 2));
        }

        private class MixedAccessPropertiesClass
        {
            public MixedAccessPropertiesClass(int publicGet)
            {
                PublicGet = publicGet;
            }

            public int PublicGet { get; private set; }
            public int PublicGetSet { get; set; }
        }

        [Test]
        public void CopyTo_DefaultsToObjectCopyScenario()
        {
            ScenarioCatcher catcher = new ScenarioCatcher();
            catcher.CopyTo<ScenarioCatcher>();
            catcher.Scenario.ShouldBe(SerializationScenario.ObjectCopy);
        }

        [Test]
        public void CopyTo_GivenScenario_OverridesScenario()
        {
            ScenarioCatcher catcher = new ScenarioCatcher();
            catcher.CopyTo<ScenarioCatcher>(scenario: "foo");
            catcher.Scenario.ShouldBe("foo");
        }

        private class ScenarioCatcher
        {
            public string Scenario;

            public int Property
            {
                get
                {
                    Scenario = SerializationScenario.Current;
                    return 1;
                }
                set { Scenario = SerializationScenario.Current; }
            }
        }
    }
}
