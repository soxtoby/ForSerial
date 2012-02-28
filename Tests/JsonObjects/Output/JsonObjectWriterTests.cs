using System.Linq;
using json.JsonObjects;
using NUnit.Framework;

namespace json.Tests.JsonObjects
{
    [TestFixture]
    public class JsonObjectWriterTests
    {
        private JsonObjectWriter sut;

        [SetUp]
        public void SetUp()
        {
            sut = new JsonObjectWriter();
        }

        [Test]
        public void WriteValue_GivenNull_CreatesNullValue()
        {
            sut.Write(null);
            sut.Result
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBeNull();
        }

        [Test]
        public void WriteValue_GivenNumber_CreatesValueWithNumber()
        {
            sut.Write(1);
            sut.Result
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(1);
        }

        [Test]
        public void WriteValue_GivenString_CreatesValueWithString()
        {
            sut.Write("foo");
            sut.Result
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe("foo");
        }

        [Test]
        public void WriteValue_GivenBoolean_CreatesValueWithBoolean()
        {
            sut.Write(true);
            sut.Result
                .ShouldBe<JsonValue>()
                .And.Value.ShouldBe(true);
        }

        [Test]
        public void EmptyStructure_CreatesEmptyMap()
        {
            sut.BeginStructure(null);
            sut.EndStructure();
            sut.Result.ShouldBe<JsonMap>()
                .And.Count.ShouldBe(0);
        }

        [Test]
        public void SinglePropertyStructure()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.EndStructure();
            sut.Result.ShouldBe<JsonMap>()
                .And["foo"].ShouldBe<JsonValue>()
                    .And.Value.ShouldBe(1);
        }

        [Test]
        public void TwoPropertyStructure()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.AddProperty("bar");
            sut.Write(2);
            sut.EndStructure();

            sut.Result.ShouldBe<JsonMap>()
                .And(map => map["foo"].ShouldBe<JsonValue>()
                    .And.Value.ShouldBe(1))
                .And(map => map["bar"].ShouldBe<JsonValue>()
                    .And.Value.ShouldBe(2));
        }

        [Test]
        public void NestedStructures()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.BeginStructure(null);
            sut.AddProperty("bar");
            sut.Write(1);
            sut.EndStructure();
            sut.AddProperty("baz");
            sut.Write(2);
            sut.EndStructure();

            sut.Result.ShouldBe<JsonMap>()
                .And(map => map["foo"].ShouldBe<JsonMap>()
                    .And.Value("bar").ShouldBe(1))
                .And(map => map["baz"].ShouldBe<JsonValue>()
                    .And.Value.ShouldBe(2));
        }

        [Test]
        public void EmptySequence()
        {
            sut.BeginSequence();
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void SingleItemSequence()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And.Values().ShouldBe(new object[] { 1 });
        }

        [Test]
        public void TwoItemSequence()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.Write(2);
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And.Values().ShouldBe(new object[] { 1, 2 });
        }

        [Test]
        public void NestedSequences()
        {
            sut.BeginSequence();
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();
            sut.Write(2);
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And.ItemsSatisfy(
                    first => first.ShouldBe<JsonArray>()
                        .And.Single().Value().ShouldBe(1),
                    second => second.Value().ShouldBe(2));
        }

        [Test]
        public void SequenceInsideStructure()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();
            sut.EndStructure();

            sut.Result.ShouldBe<JsonMap>()
                .And["foo"].ShouldBe<JsonArray>()
                    .And.Single().Value().ShouldBe(1);
        }

        [Test]
        public void StructureInsideSequence()
        {
            sut.BeginSequence();
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.EndStructure();
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And.Single().ShouldBe<JsonMap>()
                    .And["foo"].Value().ShouldBe(1);
        }

        [Test]
        public void StructureReference()
        {
            sut.BeginSequence();
            sut.BeginStructure(null);
            sut.EndStructure();
            sut.WriteReference(0);
            sut.EndSequence();

            sut.Result.ShouldBe<JsonArray>()
                .And(array => array[0].ShouldBeSameAs(array[1]));
        }
    }
}