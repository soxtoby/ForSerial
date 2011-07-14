using NUnit.Framework;

namespace json.Objects
{
    [TestFixture]
    public class TypeDefinitionTests
    {
        [Test]
        public void StringIsSerializable()
        {
            Assert.IsTrue(TypeDefinition.GetTypeDefinition(typeof(string)).IsSerializable);
        }

        [Test]
        public void StringIsDeserializable()
        {
            Assert.IsTrue(TypeDefinition.GetTypeDefinition(typeof(string)).IsDeserializable);
        }
    }
}