using System;
using System.Reflection;
using ForSerial.Objects;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class PropertyDefinitionBuilderTests
    {
        private ObjectInterfaceProvider objectInterfaceProvider;
        private PropertyDefinitionBuilder sut;
        private FieldInfo field;

        [SetUp]
        public void Initialize()
        {
            objectInterfaceProvider = Substitute.For<ObjectInterfaceProvider>();
            sut = new PropertyDefinitionBuilder(objectInterfaceProvider);
            field = Substitute.For<FieldInfo>();
            field.DeclaringType.Returns(Substitute.For<Type>());
        }

        [Test]
        public void BuildFieldDefinition_Type()
        {
            field.FieldType.Returns(typeof(string));
            sut.Build(field)
                .TypeDef.Type.ShouldBe(typeof(string));
        }

        [Test]
        public void BuildFieldDefinition_Name()
        {
            field.Name.Returns("foo");
            sut.Build(field)
                .Name.ShouldBe("foo");
        }

        [Test]
        public void BuildFieldDefinition_DeclaringType()
        {
            field.DeclaringType.FullName.Returns("foo");
            field.Name.Returns("bar");
            sut.Build(field)
                .FullName.ShouldBe("foo.bar");
        }

        [Test]
        public void BuildFieldDefinition_Public()
        {
            field.Attributes.Returns(FieldAttributes.FamANDAssem | FieldAttributes.Family); // This is what FieldInfo.IsPublic checks for
            sut.Build(field)
                .MatchesPropertyFilter(PropertyFilter.PublicGetSet)
                .ShouldBe(true);
        }

        [Test]
        public void BuildFieldDefinition_Private()
        {
            sut.Build(field)
                .MatchesPropertyFilter(PropertyFilter.PublicGetSet)
                .ShouldBe(false);
        }

        [Test]
        public void BuildFieldDefinition_CanGet()
        {
            sut.Build(field)
                .CanGet.ShouldBe(true);
        }

        [Test]
        public void BuildFieldDefinition_CanSet()
        {
            sut.Build(field)
                .CanSet.ShouldBe(true);
        }

        [Test]
        public void BuildFieldDefinition_SetOn()
        {
            object expectedTarget = new object();
            object expectedValue = new object();

            sut.Build(field)
                .SetOn(expectedTarget, expectedValue);

            objectInterfaceProvider.GetFieldSetter(field).Received()(expectedTarget, expectedValue);
        }

        [Test]
        public void BuildFieldDefinition_GetFrom()
        {
            object source = new object();
            object expectedValue = new object();

            objectInterfaceProvider.GetFieldGetter(field)(source).Returns(expectedValue);

            object result = sut.Build(field)
                .GetFrom(source);

            result.ShouldBeSameAs(expectedValue);
        }
    }
}