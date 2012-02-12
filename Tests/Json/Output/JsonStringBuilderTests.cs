using NUnit.Framework;
using json.Json;
using json.Objects;
using json.Tests.Output;

namespace json.Tests.Json.Output
{
    [TestFixture]
    public class JsonStringBuilderTests
    {
        [Test]
        public void StringIsEscaped()
        {
            json.Output parseString = JsonStringBuilder.Default.CreateValue("\"foo\\bar\"");
            Assert.AreEqual(@"{""value"":""\""foo\\bar\""""}", JsonStringBuilder.GetResult(parseString.AsStructure()));
        }

        [Test]
        public void NumberIsWrappedInObject()
        {
            json.Output number = JsonStringBuilder.Default.CreateValue(5);
            Assert.AreEqual("{\"value\":5}", JsonStringBuilder.GetResult(number.AsStructure()));
        }

        [Test]
        public void StringIsWrappedInObject()
        {
            json.Output str = JsonStringBuilder.Default.CreateValue("foo");
            Assert.AreEqual("{\"value\":\"foo\"}", JsonStringBuilder.GetResult(str.AsStructure()));
        }

        [Test]
        public void ArrayIsWrappedInObject()
        {
            SequenceOutput array = JsonStringBuilder.Default.CreateSequence();
            Assert.AreEqual("{\"items\":[]}", JsonStringBuilder.GetResult(array.AsStructure()));
        }

        [Test]
        public void MaintainSingleReference()
        {
            string json = Convert.From.Object(new SameReferenceTwice(new { foo = 5 }), new ObjectParsingOptions { SerializeAllTypes = true })
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", json);
        }

        [Test]
        public void MaintainTwoReferences()
        {
            string json = Convert.From.Object(new TwoReferencesTwice(new { foo = 5 }, new { bar = 6 }), new ObjectParsingOptions { SerializeAllTypes = true })
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""bar"":6},""Three"":{""_ref"":1},""Four"":{""_ref"":2}}", json);
        }

        [Test]
        [ExpectedException(typeof(JsonStringBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            JsonStringBuilder.GetResult(NullOutputStructure.Instance);
        }
    }
}

