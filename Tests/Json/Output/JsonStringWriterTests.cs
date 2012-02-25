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
            sut.WriteValue(1);
            Json.ShouldBe("1");
        }

        [Test]
        public void WriteValue_GivenString_WrapsStringInQuotes()
        {
            sut.WriteValue("foo");
            Json.ShouldBe(@"""foo""");
        }

        [Test]
        public void WriteValue_GivenStringWithQuotes_EscapesQuotes()
        {
            sut.WriteValue(@"foo""bar");
            Json.ShouldBe(@"""foo\""bar""");
        }

        [Test]
        public void WriteValue_GivenStringWithBackslash_EscapesBackslash()
        {
            sut.WriteValue("foo\\bar");
            Json.ShouldBe(@"""foo\\bar""");
        }

        [Test]
        public void WriteValue_GivenTrue_WritesTrue()
        {
            sut.WriteValue(true);
            Json.ShouldBe("true");
        }

        [Test]
        public void WriteValue_GivenFalse_WritesFalse()
        {
            sut.WriteValue(false);
            Json.ShouldBe("false");
        }

        [Test]
        public void WriteValue_GivenNull_WritesNull()
        {
            sut.WriteValue(null);
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
            sut.WriteValue("bar");
            sut.EndStructure();
            Json.ShouldBe(@"{""foo"":""bar""}");
        }

        [Test]
        public void TwoPropertyStructure_SecondPropertyDelimited()
        {
            sut.BeginStructure();
            sut.AddProperty("foo");
            sut.WriteValue(1);
            sut.AddProperty("bar");
            sut.WriteValue(2);
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
            sut.WriteValue(1);
            sut.EndSequence();
            Json.ShouldBe("[1]");
        }

        [Test]
        public void TwoItemSequence_SecondItemDelimited()
        {
            sut.BeginSequence();
            sut.WriteValue(1);
            sut.WriteValue(2);
            sut.EndSequence();
            Json.ShouldBe("[1,2]");
        }
    }
}