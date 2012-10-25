using System.Linq;
using EasyAssertions;
using ForSerial.JsonObjects;
using NUnit.Framework;

namespace ForSerial.Tests.JsonObjects
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
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBeNull();
        }

        [Test]
        public void WriteValue_GivenNumber_CreatesValueWithNumber()
        {
            sut.Write(1);
            sut.Result
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(1);
        }

        [Test]
        public void WriteValue_GivenString_CreatesValueWithString()
        {
            sut.Write("foo");
            sut.Result
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe("foo");
        }

        [Test]
        public void WriteValue_GivenBoolean_CreatesValueWithBoolean()
        {
            sut.Write(true);
            sut.Result
                .ShouldBeA<JsonValue>()
                .And.Value.ShouldBe(true);
        }

        [Test]
        public void EmptyStructure_CreatesEmptyMap()
        {
            sut.BeginStructure(null);
            sut.EndStructure();
            sut.Result.ShouldBeA<JsonMap>()
                .And.Count.ShouldBe(0);
        }

        [Test]
        public void SinglePropertyStructure()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.EndStructure();
            sut.Result.ShouldBeA<JsonMap>()
                .And["foo"].ShouldBeA<JsonValue>()
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

            sut.Result.ShouldBeA<JsonMap>()
                .And(map => map["foo"].ShouldBeA<JsonValue>()
                    .And.Value.ShouldBe(1))
                .And(map => map["bar"].ShouldBeA<JsonValue>()
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

            sut.Result.ShouldBeA<JsonMap>()
                .And(map => map["foo"].ShouldBeA<JsonMap>()
                    .And.Value("bar").ShouldBe(1))
                .And(map => map["baz"].ShouldBeA<JsonValue>()
                    .And.Value.ShouldBe(2));
        }

        [Test]
        public void EmptySequence()
        {
            sut.BeginSequence();
            sut.EndSequence();

            sut.Result.ShouldBeA<JsonArray>()
                .And.ShouldBeEmpty();
        }

        [Test]
        public void SingleItemSequence()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();

            sut.Result.ShouldBeA<JsonArray>()
                .And.Values().ShouldMatch(new object[] { 1 });
        }

        [Test]
        public void TwoItemSequence()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.Write(2);
            sut.EndSequence();

            sut.Result.ShouldBeA<JsonArray>()
                .And.Values().ShouldMatch(new object[] { 1, 2 });
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

            sut.Result.ShouldBeA<JsonArray>()
                .And.ItemsSatisfy(
                    first => first.ShouldBeA<JsonArray>()
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

            sut.Result.ShouldBeA<JsonMap>()
                .And["foo"].ShouldBeA<JsonArray>()
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

            sut.Result.ShouldBeA<JsonArray>()
                .And.Single().ShouldBeA<JsonMap>()
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

            sut.Result.ShouldBeA<JsonArray>()
                .And(array => array[0].ShouldReferTo(array[1]));
        }
    }
}