using EasyAssertions;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class SerializeAsObjectAttributeTests
    {
        [Test]
        public void Filter_LeavesNonSequenceDefinitionFactoryMethod()
        {
            new SerializeAsObjectAttribute()
                .Filter(new FactoryMethod[] { DefaultStructureDefinition.CreateDefaultStructureDefinition })
                .ShouldNotBeEmpty();
        }

        [Test]
        public void Filter_FiltersOutSequenceDefinitionFactoryMethod()
        {
            new SerializeAsObjectAttribute()
                .Filter(new FactoryMethod[] { CollectionDefinition.CreateCollectionDefinition })
                .ShouldBeEmpty();
        }
    }
}