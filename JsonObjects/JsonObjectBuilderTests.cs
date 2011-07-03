﻿using NUnit.Framework;

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
    }
}