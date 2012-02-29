using json.Objects;
using NUnit.Framework;

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
    }
}