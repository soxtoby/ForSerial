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
            ParseString parseString = JsonStringBuilder.Default.CreateString("\"foo\\bar\"");
            StringValueObject obj = new StringValueObject();
            parseString.AddToObject(obj, null);
            Assert.AreEqual(@"\""foo\\bar\""", obj.StringValue);
        }

        private class StringValueObject : NullParseObject
        {
            public string StringValue { get; private set; }

            public override void AddString(string name, string value)
            {
                StringValue = value;
            }
        }

        [Test]
        public void Number_IsWrappedInObject()
        {
            ParseNumber number = JsonStringBuilder.Default.CreateNumber(5);
            Assert.AreEqual("{\"value\":5}", JsonStringBuilder.GetResult(number.AsObject()));
        }

        [Test]
        public void String_IsWrappedInObject()
        {
            ParseString str = JsonStringBuilder.Default.CreateString("foo");
            Assert.AreEqual("{\"value\":\"foo\"}", JsonStringBuilder.GetResult(str.AsObject()));
        }

        [Test]
        public void Array_IsWrappedInObject()
        {
            ParseArray array = JsonStringBuilder.Default.CreateArray();
            Assert.AreEqual("{\"items\":[]}", JsonStringBuilder.GetResult(array.AsObject()));
        }

        [Test]
        public void MaintainSingleReference()
        {
            string json = Parse.From.Object(new SameReferenceTwice(new { foo = 5 }), ObjectParser.Options.SerializeAllTypes)
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""_ref"":1}}", json);
        }

        [Test]
        public void MaintainTwoReferences()
        {
            string json = Parse.From.Object(new TwoReferencesTwice(new { foo = 5 }, new { bar = 6 }), ObjectParser.Options.SerializeAllTypes)
                .ToJson(JsonStringBuilder.Options.MaintainObjectReferences);
            Assert.AreEqual(@"{""One"":{""foo"":5},""Two"":{""bar"":6},""Three"":{""_ref"":1},""Four"":{""_ref"":2}}", json);
        }

        [Test]
        [ExpectedException(typeof(JsonStringBuilder.InvalidResultObject))]
        public void InvalidResultObject()
        {
            JsonStringBuilder.GetResult(NullParseObject.Instance);
        }

        [Test]
        [ExpectedException(typeof(JsonStringBuilder.CannotAddValueToReference))]
        public void AddValueToReferenceObject()
        {
            var builder = new JsonStringBuilder(JsonStringBuilder.Options.MaintainObjectReferences);
            var originalObject = builder.CreateObject();
            var reference = builder.CreateReference(originalObject);
            reference.AddNull("foo");
        }

    }
}

