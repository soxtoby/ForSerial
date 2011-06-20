using NUnit.Framework;

namespace json.Json
{
    [TestFixture]
    public class JsonStringBuilderTests
    {
        [Test]
        public void StringIsEscaped()
        {
            ParseString parseString = JsonStringBuilder.Instance.CreateString("\"foo\\bar\"");
            StringValueObject obj = new StringValueObject();
            parseString.AddToObject(obj, null);
            Assert.AreEqual(@"\""foo\\bar\""", obj.StringValue);
        }

        private class StringValueObject : TestParseObject
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
            ParseNumber number = JsonStringBuilder.Instance.CreateNumber(5);
            Assert.AreEqual("{\"value\":5}", JsonStringBuilder.GetResult(number.AsObject()));
        }

        [Test]
        public void String_IsWrappedInObject()
        {
            ParseString str = JsonStringBuilder.Instance.CreateString("foo");
            Assert.AreEqual("{\"value\":\"foo\"}", JsonStringBuilder.GetResult(str.AsObject()));
        }

        [Test]
        public void Array_IsWrappedInObject()
        {
            ParseArray array = JsonStringBuilder.Instance.CreateArray();
            Assert.AreEqual("{\"items\":[]}", JsonStringBuilder.GetResult(array.AsObject()));
        }
    }
}

