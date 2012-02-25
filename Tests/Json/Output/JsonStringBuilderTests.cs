using json.Json;
using json.Objects;
using NUnit.Framework;

namespace json.Tests.Json
{
    [TestFixture]
    public class JsonStringBuilderTests
    {
        [Test]
        public void StringIsEscaped()
        {
            Output parseString = JsonStringBuilder.GetDefault().CreateValue("\"foo\\bar\"");
            Assert.AreEqual(@"{""value"":""\""foo\\bar\""""}", JsonStringBuilder.GetResult(parseString.AsStructure()));
        }

        [Test]
        public void NumberIsWrappedInObject()
        {
            Output number = JsonStringBuilder.GetDefault().CreateValue(5);
            Assert.AreEqual("{\"value\":5}", JsonStringBuilder.GetResult(number.AsStructure()));
        }

        [Test]
        public void StringIsWrappedInObject()
        {
            Output str = JsonStringBuilder.GetDefault().CreateValue("foo");
            Assert.AreEqual("{\"value\":\"foo\"}", JsonStringBuilder.GetResult(str.AsStructure()));
        }

        [Test]
        public void ArrayIsWrappedInObject()
        {
            JsonStringBuilder sut = JsonStringBuilder.GetDefault();
            SequenceOutput array = sut.BeginSequence();
            sut.EndSequence();
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


