using json.Objects;
using NUnit.Framework;

namespace json.Json
{
    [TestFixture]
    public class JsonStringBuilderTests
    {
        [Test]
        public void StringIsEscaped()
        {
            ParseValue parseString = JsonStringBuilder.Default.CreateValue("\"foo\\bar\"");
            Assert.AreEqual(@"{""value"":""\""foo\\bar\""""}", JsonStringBuilder.GetResult(parseString.AsObject()));
        }

        [Test]
        public void NumberIsWrappedInObject()
        {
            ParseValue number = JsonStringBuilder.Default.CreateValue(5);
            Assert.AreEqual("{\"value\":5}", JsonStringBuilder.GetResult(number.AsObject()));
        }

        [Test]
        public void StringIsWrappedInObject()
        {
            ParseValue str = JsonStringBuilder.Default.CreateValue("foo");
            Assert.AreEqual("{\"value\":\"foo\"}", JsonStringBuilder.GetResult(str.AsObject()));
        }

        [Test]
        public void ArrayIsWrappedInObject()
        {
            ParseArray array = JsonStringBuilder.Default.CreateArray();
            Assert.AreEqual("{\"items\":[]}", JsonStringBuilder.GetResult(array.AsObject()));
        }

        [Test]
        public void MaintainSingleReference()
        {
            string json = Parse.From.Object(new SameReferenceTwice(new { foo = 5 }), new CustomObjectParsingOptions { SerializeAllTypes = true })
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", json);
        }

        [Test]
        public void MaintainTwoReferences()
        {
            string json = Parse.From.Object(new TwoReferencesTwice(new { foo = 5 }, new { bar = 6 }), new CustomObjectParsingOptions { SerializeAllTypes = true })
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""bar"":6},""Three"":{""_ref"":1},""Four"":{""_ref"":2}}", json);
        }

        [Test]
        [ExpectedException(typeof(JsonStringBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            JsonStringBuilder.GetResult(NullParseObject.Instance);
        }
    }
}

