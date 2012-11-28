using System;
using System.Collections.Generic;
using EasyAssertions;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class CollectionDefinitionTests
    {
        [Test]
        public void ConstructNew_NoDefaultConstructor_Throws()
        {
            CollectionDefinition sut = (CollectionDefinition)CollectionDefinition.CreateCollectionDefinition(typeof(NoDefaultConstructorClass));

            Should.Throw<TypeDefinition.NoDefaultConstructor>(() => sut.ConstructNew());
        }

        private class NoDefaultConstructorClass : List<int>
        {
            public NoDefaultConstructorClass(int value) { }
        }
    }
}