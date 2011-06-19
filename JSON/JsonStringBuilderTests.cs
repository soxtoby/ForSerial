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
            TestObject obj = new TestObject();
            parseString.AddToObject(obj, null);
            Assert.AreEqual(@"\""foo\\bar\""", obj.StringValue);
        }

        private class TestObject : ParseObject
        {
            public string TypeIdentifier { get; private set; }
            public string StringValue { get; private set; }

            public bool SetType(string typeIdentifier, Parser parser)
            {
                TypeIdentifier = typeIdentifier;
                return false;
            }

            public void AddNull(string name)
            {
                throw new System.NotImplementedException();
            }

            public void AddBoolean(string name, bool value)
            {
                throw new System.NotImplementedException();
            }

            public void AddNumber(string name, double value)
            {
                throw new System.NotImplementedException();
            }

            public void AddString(string name, string value)
            {
                StringValue = value;
            }

            public void AddObject(string name, ParseObject value)
            {
                throw new System.NotImplementedException();
            }

            public void AddArray(string name, ParseArray value)
            {
                throw new System.NotImplementedException();
            }

            public void AddToObject(ParseObject obj, string name)
            {
                throw new System.NotImplementedException();
            }

            public void AddToArray(ParseArray array)
            {
                throw new System.NotImplementedException();
            }

            public ParseObject AsObject()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

