using EasyAssertions;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class DefaultObjectStructureTests
    {
        [Test]
        public void PreferredConstructorUsedOverPreviousConstructor()
        {
            DefaultObjectStructure sut = new DefaultObjectStructure((StructureDefinition)TypeCache.GetTypeDefinition(typeof(PreferredConstructorClass)));
            SetProperty(sut, "one", 1);
            SetProperty(sut, "two", "two");

            sut.GetTypedValue()
                .ShouldBeA<PreferredConstructorClass>()
                .And(c => c.One.ShouldBe(0))
                .And(c => c.Two.ShouldBe("two"));
        }

        private class PreferredConstructorClass
        {
            public readonly string Two;
            public readonly int One;

            public PreferredConstructorClass(int one)
            {
                One = one;
            }

            [SerializationConstructor]
            public PreferredConstructorClass(string two)
            {
                Two = two;
            }
        }

        private void SetProperty(DefaultObjectStructure sut, string name, object value)
        {
            sut.SetCurrentProperty(name);
            sut.Add(new DefaultObjectValue(value));
        }
    }
}