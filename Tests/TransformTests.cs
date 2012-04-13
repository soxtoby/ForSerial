using System;
using System.Collections.Generic;
using json.Objects;
using NUnit.Framework;

namespace json.Tests
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
            new[] { 1 }.ToFormattedJson(null, "_")
                .ShouldBe("[" + lf + "_1" + lf + "]");
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
        public void CopyTo_CopiesToNewObject()
        {
            List<int> original = new List<int> { 1 };
            original.CopyTo<List<int>>()
                .ShouldNotBeSameAs(original)
                .And.ShouldMatch(new[] { 1 });
        }

        [Test]
        public void CopyTo_PassesOptionsToObjectReader()
        {
            new MixedAccessPropertiesClass(1) { PublicGetSet = 2 }
                .CopyTo<Dictionary<string, int>>(new ObjectParsingOptions { PropertyFilter = PropertyFilter.PublicGetSet })
                .ShouldMatch(new Dictionary<string, int> { { "PublicGetSet", 2 } });
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
    }
}
