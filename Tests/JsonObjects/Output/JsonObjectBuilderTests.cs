using json.Objects;
using NUnit.Framework;

namespace json.JsonObjects
{
    [TestFixture]
    public class JsonObjectBuilderTests
    {
        [Test]
        public void MaintainSingleReference()
        {
            JsonObject jsonObject = Convert.From.Object(new SameReferenceTwice(new { foo = 5 }), new ObjectParsingOptions { SerializeAllTypes = true }).ToJsonObject();
            Assert.AreSame(jsonObject["One"], jsonObject["Two"]);
        }

        [Test]
        public void MaintainTwoReferences()
        {
            JsonObject jsonObject = Convert.From
                .Object(new TwoReferencesTwice(new { foo = 5 }, new { bar = 6 }), new ObjectParsingOptions { SerializeAllTypes = true })
                .ToJsonObject();
            Assert.AreSame(jsonObject["One"], jsonObject["Three"]);
            Assert.AreSame(jsonObject["Two"], jsonObject["Four"]);
        }

        [Test]
        [ExpectedException(typeof(JsonObjectBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            JsonObjectBuilder.GetResult(NullOutputStructure.Instance);
        }
    }
}
