using NUnit.Framework;
using json.Objects;

namespace json.Tests.Objects
{
    [TestFixture]
    public class TypeDefinitionTests
    {
        [Test]
        public void StringIsSerializable()
        {
            Assert.IsTrue(CurrentTypeHandler.GetTypeDefinition(typeof(string)).IsSerializable);
        }

        [Test]
        public void StringIsDeserializable()
        {
            Assert.IsTrue(CurrentTypeHandler.GetTypeDefinition(typeof(string)).IsDeserializable);
        }
    }
}