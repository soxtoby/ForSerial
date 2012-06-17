using System.Collections.Generic;
using EasyAssertions;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class TypeDefinitionFactoryTests
    {
        [Test]
        public void SequenceDefinitionCreatedForSequence()
        {
            TypeDefinitionFactory.CreateTypeDefinition(typeof(Sequence))
                .ShouldBeA<SequenceDefinition>();
        }

        [Test]
        public void FactoryMethodsFilteredByAttribute()
        {
            TypeDefinitionFactory.CreateTypeDefinition(typeof(SequenceWithAttribute))
                .ShouldBeA<StructureDefinition>();
        }

        private class Sequence : List<int> { }

        [SerializeAsObject]
        private class SequenceWithAttribute : List<int> { }
    }
}