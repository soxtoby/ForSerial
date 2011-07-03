using NUnit.Framework;

namespace json.JsonObjects
{
    [TestFixture]
    public class JsonObjectBuilderTests
    {
        [Test]
        public void MaintainSingleReference()
        {
            JsonObject jsonObject = Parse.From.Object(new SameReferenceTwice(new { foo = 5 })).ToJsonObject();
            Assert.AreSame(jsonObject["One"], jsonObject["Two"]);
        }

        [Test]
        public void MaintainTwoReferences()
        {
            JsonObject jsonObject = Parse.From.Object(new TwoReferencesTwice(new { foo = 5 }, new { bar = 6 })).ToJsonObject();
            Assert.AreSame(jsonObject["One"], jsonObject["Three"]);
            Assert.AreSame(jsonObject["Two"], jsonObject["Four"]);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToObject()
        {
            ParseObject parseObject = JsonObjectBuilder.Instance.CreateObject();
            parseObject.AddObject("foo", NullParseObject.Instance);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.UnsupportedParseObject))]
        public void AddUnsupportedParseObjectToArray()
        {
            ParseArray parseArray = JsonObjectBuilder.Instance.CreateArray();
            parseArray.AddObject(NullParseObject.Instance);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToObject()
        {
            ParseObject parseObject = JsonObjectBuilder.Instance.CreateObject();
            parseObject.AddArray("foo", NullParseArray.Instance);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.UnsupportedParseArray))]
        public void AddUnsupportedParseArrayToArray()
        {
            ParseArray parseArray = JsonObjectBuilder.Instance.CreateArray();
            parseArray.AddArray(NullParseArray.Instance);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            JsonObjectBuilder.GetResult(NullParseObject.Instance);
        }
    }
}
