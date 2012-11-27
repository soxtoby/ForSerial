using EasyAssertions;
using ForSerial.Objects;
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

        [Test]
        public void ConstructorParamsMatchedToFieldsWithUnderscorePrefix()
        {
            DefaultObjectStructure sut = new DefaultObjectStructure((StructureDefinition)TypeCache.GetTypeDefinition(typeof(UnderscorePrefixedFieldClass)));
            SetProperty(sut, "_field", 1);

            sut.GetTypedValue()
                .ShouldBeA<UnderscorePrefixedFieldClass>()
                .And._field.ShouldBe(1);
        }

        private class UnderscorePrefixedFieldClass
        {
            public readonly int _field;

            public UnderscorePrefixedFieldClass(int field)
            {
                _field = field;
            }
        }

        [Test]
        public void PropertiesPassedIntoConstructorAreNotRepopulated()
        {
            DefaultObjectStructure sut = new DefaultObjectStructure((StructureDefinition)TypeCache.GetTypeDefinition(typeof(PropertyConstructorClass)));
            SetProperty(sut, "Property", 1);

            sut.GetTypedValue()
                .ShouldBeA<PropertyConstructorClass>()
                .And(o => o.PropertyViaConstructor.ShouldBe(1))
                .And(o => o.Property.ShouldBe(0));
        }

        private class PropertyConstructorClass
        {
            public int PropertyViaConstructor;
            public int Property;

            public PropertyConstructorClass(int property)
            {
                PropertyViaConstructor = property;
            }
        }

        private static void SetProperty(DefaultObjectStructure sut, string name, object value)
        {
            sut.SetCurrentProperty(name);
            sut.Add(new DefaultObjectValue(value));
        }
    }
}