using System.IO;
using json.Json;
using NUnit.Framework;

namespace json.Tests.Json
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
        public void WriteValue_GivenNumber_WritesNumber()
        {
            sut.Write(1);
            Json.ShouldBe("1");
        }

        [Test]
        public void WriteValue_GivenString_WrapsStringInQuotes()
        {
            sut.Write("foo");
            Json.ShouldBe(@"""foo""");
        }

        [Test]
        public void WriteValue_GivenStringWithQuotes_EscapesQuotes()
        {
            sut.Write(@"foo""bar");
            Json.ShouldBe(@"""foo\""bar""");
        }

        [Test]
        public void WriteValue_GivenStringWithBackslash_EscapesBackslash()
        {
            sut.Write("foo\\bar");
            Json.ShouldBe(@"""foo\\bar""");
        }

        [Test]
        public void WriteValue_GivenTrue_WritesTrue()
        {
            sut.Write(true);
            Json.ShouldBe("true");
        }

        [Test]
        public void WriteValue_GivenFalse_WritesFalse()
        {
            sut.Write(false);
            Json.ShouldBe("false");
        }

        [Test]
        public void WriteValue_GivenNull_WritesNull()
        {
            sut.Write(null);
            Json.ShouldBe("null");
        }

        [Test]
        public void EmptyStructure()
        {
            sut.BeginStructure();
            sut.EndStructure();
            Json.ShouldBe("{}");
        }

        [Test]
        public void SinglePropertyStructure()
        {
            sut.BeginStructure();
            sut.AddProperty("foo");
            sut.Write("bar");
            sut.EndStructure();
            Json.ShouldBe(@"{""foo"":""bar""}");
        }

        [Test]
        public void TwoPropertyStructure_SecondPropertyDelimited()
        {
            sut.BeginStructure();
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
            sut.BeginStructure();
            sut.EndStructure();
            sut.BeginStructure();
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