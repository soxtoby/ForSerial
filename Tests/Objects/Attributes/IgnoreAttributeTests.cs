using ForSerial.Objects;
using NSubstitute;
using NUnit.Framework;
using IgnoreAttribute = ForSerial.Objects.IgnoreAttribute;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class IgnoreAttributeTests : AttributeTests<IgnoreAttribute>
    {
        private const string IgnoreScenario = "TestScenario";

        [Test]
        public void CanGet_WrongScenario_PassesThrough()
        {
            TestCanGetOverride(null, true, true);
        }

        [Test]
        public void CanGet_IgnoreScenario_CannotGet()
        {
            TestCanGetOverride(IgnoreScenario, true, false);
        }

        [Test]
        public void CanSet_WrongScenario_PassesThrough()
        {
            TestCanSetOverride(null, true, true);
        }

        [Test]
        public void CanSet_IgnoreScenario_CannotSet()
        {
            TestCanSetOverride(IgnoreScenario, true, false);
        }

        [Test]
        public void Read_WrongScenario_PassesThrough()
        {
            TestReadOverride(null,
                sut => sut.InnerDefinition.Received().Read(Arg.Any<object>(), Arg.Any<ObjectReader>(), Arg.Any<Writer>()));
        }

        [Test]
        public void Read_IgnoreScenario_InnerPropertyIsNotRead()
        {
            TestReadOverride(IgnoreScenario,
                sut => sut.InnerDefinition.DidNotReceive().Read(Arg.Any<object>(), Arg.Any<ObjectReader>(), Arg.Any<Writer>()));
        }

        [Test]
        public void CanCreateValue_WrongScenario_PassesThrough()
        {
            TestCanCreateValueOverride(null, true);
        }

        [Test]
        public void CanCreateValue_IgnoreScenario_CannotCreateValue()
        {
            TestCanCreateValueOverride(IgnoreScenario, false);
        }

        [Test]
        public void CreateSequence_WrongScenario_PassesThrough()
        {
            ObjectContainer expectedSequence = Substitute.For<ObjectContainer>();
            TestCreateSequenceOverride(null, expectedSequence,
                sut => sut.CreateSequence().ShouldBe(expectedSequence));
        }

        [Test]
        public void CreateSequence_IgnoreScenario_ReturnsNullSequence()
        {
            TestCreateSequenceOverride(IgnoreScenario, null,
                sut => sut.CreateSequence().ShouldBe(NullObjectSequence.Instance));
        }

        [Test]
        public void CreateDefaultStructure_WrongScenario_PassesThrough()
        {
            ObjectContainer expectedStructure = Substitute.For<ObjectContainer>();
            TestCreateDefaultStructureOverride(null, expectedStructure, expectedStructure);
        }

        [Test]
        public void CreateDefaultStructure_IgnoreScenario_ReturnsNullStructure()
        {
            TestCreateDefaultStructureOverride(IgnoreScenario, null, NullObjectStructure.Instance);
        }

        [Test]
        public void CreateTypedStructure_WrongScenario_PassesThrough()
        {
            ObjectContainer expectedStructure = Substitute.For<ObjectContainer>();
            TestCreateTypedStructureOverride(null, expectedStructure, expectedStructure);
        }

        [Test]
        public void CreateTypedStructure_IgnoreScenario_ReturnsNullStructure()
        {
            TestCreateTypedStructureOverride(IgnoreScenario, null, NullObjectStructure.Instance);
        }

        [Test]
        public void CreateValue_WrongScenario_PassesThrough()
        {
            ObjectOutput expectedValue = Substitute.For<ObjectOutput>();
            TestCreateValueOverride(null, expectedValue, expectedValue);
        }

        [Test]
        public void CreateValue_IgnoreScenario_ReturnsNullValue()
        {
            TestCreateValueOverride(IgnoreScenario, null, NullObjectValue.Instance);
        }

        [Test]
        public void MatchesPropertyFilter_WrongScenario_PassesThrough()
        {
            TestMatchesPropertyFilterOverride(null, PropertyFilter.PublicGet, true, true);
        }

        [Test]
        public void MatchesPropertyFilter_IgnoreScenario_DoesNotMatch()
        {
            TestMatchesPropertyFilterOverride(IgnoreScenario, PropertyFilter.PublicGet, true, false);
        }

        protected override IgnoreAttribute CreateAttribute()
        {
            return new IgnoreAttribute(IgnoreScenario);
        }
    }

    [TestFixture]
    public class CopyIgnoreAttributeTests : AttributeTests<CopyIgnoreAttribute>
    {
        [Test]
        public void CanGet_WrongScenario_PassesThrough()
        {
            TestCanGetOverride(null, true, true);
        }

        [Test]
        public void CanGet_ObjectCopy_CannotGet()
        {
            TestCanGetOverride(SerializationScenario.ObjectCopy, true, false);
        }

        protected override CopyIgnoreAttribute CreateAttribute()
        {
            return new CopyIgnoreAttribute();
        }
    }

    [TestFixture]
    public class JsonIgnoreAttributeTests : AttributeTests<JsonIgnoreAttribute>
    {
        [Test]
        public void CanGet_WrongScenario_PassesThrough()
        {
            TestCanGetOverride(null, true, true);
        }

        [Test]
        public void CanGet_SerializeToJson_CannotGet()
        {
            TestCanGetOverride(SerializationScenario.SerializeToJson, true, false);
        }

        protected override JsonIgnoreAttribute CreateAttribute()
        {
            return new JsonIgnoreAttribute();
        }
    }
}