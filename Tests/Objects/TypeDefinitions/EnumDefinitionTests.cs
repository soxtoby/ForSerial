using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;
using NSubstitute;
using NUnit.Framework;
using EasyAssertions;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class EnumDefinitionTests
    {
        private EnumDefinition sut;
        private Writer writer;

        [SetUp]
        public void SetUp()
        {
            sut = EnumDefinition.CreateEnumDefinition(typeof(TestEnum));
            writer = Substitute.For<Writer>();
        }

        [Test]
        public void CreateEnumDefinition_ByteEnum()
        {
            EnumDefinition.CreateEnumDefinition(typeof(ByteEnum))
                .ShouldBeA<EnumDefinition>();
        }

        private enum ByteEnum : byte
        {
            One = 1,
            Two = 2
        }

        [Test]
        public void Read_ToInteger()
        {
            sut.Read(TestEnum.Two, null, writer, new PartialOptions { EnumSerialization = EnumSerialization.AsInteger });

            writer.Received(1).Write(2);
        }

        [Test]
        public void Read_ToString()
        {
            sut.Read(TestEnum.Two, null, writer, new PartialOptions { EnumSerialization = EnumSerialization.AsString });

            writer.Received(1).Write("Two");
        }

        [Test]
        public void CreateValue_FromDouble()
        {
            sut.CreateValue(2d)
                .GetTypedValue().ShouldBe(TestEnum.Two);
        }

        [Test]
        public void CreateValue_FromString()
        {
            sut.CreateValue("Two")
                .GetTypedValue().ShouldBe(TestEnum.Two);
        }

        private enum TestEnum
        {
            One = 1,
            Two = 2
        }
    }
}