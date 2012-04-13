using System;
using System.IO;
using ForSerial.Json;
using ForSerial.Objects;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.Json
{
    [TestFixture]
    public class JsonStringWriterTests
    {
        private JsonStringWriter sut;
        private StringWriter stringWriter;
        private string Json { get { return stringWriter.ToString(); } }

        [SetUp]
        public void Initialize()
        {
            stringWriter = new StringWriter();
            sut = new JsonStringWriter(stringWriter);
        }

        [Test]
        public void Write_GivenNumber_WritesNumber()
        {
            sut.Write(1);
            Json.ShouldBe("1");
        }

        [Test]
        public void Write_GivenString_WrapsStringInQuotes()
        {
            sut.Write("foo");
            Json.ShouldBe(@"""foo""");
        }

        [Test]
        public void Write_GivenStringWithQuotes_EscapesQuotes()
        {
            sut.Write(@"foo""bar");
            Json.ShouldBe(@"""foo\""bar""");
        }

        [Test]
        public void Write_GivenStringWithBackslash_EscapesBackslash()
        {
            sut.Write("foo\\bar");
            Json.ShouldBe(@"""foo\\bar""");
        }

        [Test]
        public void Write_GivenTrue_WritesTrue()
        {
            sut.Write(true);
            Json.ShouldBe("true");
        }

        [Test]
        public void Write_GivenFalse_WritesFalse()
        {
            sut.Write(false);
            Json.ShouldBe("false");
        }

        [Test]
        public void WriteNull()
        {
            sut.WriteNull();
            Json.ShouldBe("null");
        }

        [Test]
        public void Write_Char()
        {
            WriteCallsTextWriter(writer => writer.Write('a'), writer => writer.Write('a'));
        }

        [Test]
        public void Write_Decimal()
        {
            WriteCallsTextWriter(writer => writer.Write(1m), writer => writer.Write(1m));
        }

        [Test]
        public void Write_Double()
        {
            WriteCallsTextWriter(writer => writer.Write(1d), writer => writer.Write(1d));
        }

        [Test]
        public void Write_Float()
        {
            WriteCallsTextWriter(writer => writer.Write(1f), writer => writer.Write(1f));
        }

        [Test]
        public void Write_Int()
        {
            WriteCallsTextWriter(writer => writer.Write(1), writer => writer.Write(1));
        }

        [Test]
        public void Write_Long()
        {
            WriteCallsTextWriter(writer => writer.Write(1L), writer => writer.Write(1L));
        }

        [Test]
        public void Write_UInt()
        {
            WriteCallsTextWriter(writer => writer.Write(1U), writer => writer.Write(1U));
        }

        [Test]
        public void Write_ULong()
        {
            WriteCallsTextWriter(writer => writer.Write(1UL), writer => writer.Write(1UL));
        }

        [Test]
        public void Write_Enum()
        {
            WriteCallsTextWriter(writer => writer.Write(TypeCodeType.Boolean), writer => writer.Write((int)TypeCodeType.Boolean));
        }

        private static void WriteCallsTextWriter(Action<JsonStringWriter> jsonWriterCall, Action<TextWriter> expectedTextWriterCall)
        {
            TextWriter textWriter = Substitute.For<TextWriter>();
            var sut = new JsonStringWriter(textWriter);
            jsonWriterCall(sut);
            expectedTextWriterCall(textWriter.Received());
        }

        [Test]
        public void EmptyStructure()
        {
            sut.BeginStructure(null);
            sut.EndStructure();
            Json.ShouldBe("{}");
        }

        [Test]
        public void SinglePropertyStructure()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write("bar");
            sut.EndStructure();
            Json.ShouldBe(@"{""foo"":""bar""}");
        }

        [Test]
        public void TwoPropertyStructure_SecondPropertyDelimited()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.AddProperty("bar");
            sut.Write(2);
            sut.EndStructure();
            Json.ShouldBe(@"{""foo"":1,""bar"":2}");
        }

        [Test]
        public void EmptySequence()
        {
            sut.BeginSequence();
            sut.EndSequence();
            Json.ShouldBe("[]");
        }

        [Test]
        public void SingleItemSequence()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();
            Json.ShouldBe("[1]");
        }

        [Test]
        public void ManyItemSequence_ItemsAreDelimited()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.Write(2);
            sut.Write(3);
            sut.EndSequence();
            Json.ShouldBe("[1,2,3]");
        }

        [Test]
        public void NestedSequences()
        {
            sut.BeginSequence();
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();
            sut.BeginSequence();
            sut.Write(2);
            sut.EndSequence();
            sut.EndSequence();
            Json.ShouldBe("[[1],[2]]");
        }

        [Test]
        public void StructuresInsideSequence()
        {
            sut.BeginSequence();
            sut.BeginStructure(null);
            sut.EndStructure();
            sut.BeginStructure(null);
            sut.EndStructure();
            sut.EndSequence();
            Json.ShouldBe("[{},{}]");
        }

        [Test]
        public void WriteReference_GivenIndex_WritesReferenceObjectWithIndex()
        {
            sut.WriteReference(1);
            Json.ShouldBe(@"{""_ref"":1}");
        }
    }
}