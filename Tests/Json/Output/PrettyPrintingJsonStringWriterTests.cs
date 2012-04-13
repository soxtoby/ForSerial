using System;
using System.IO;
using ForSerial.Json;
using NUnit.Framework;

namespace ForSerial.Tests.Json
{
    [TestFixture]
    public class PrettyPrintingJsonStringWriterTests
    {
        private PrettyPrintingJsonStringWriter sut;
        private StringWriter stringWriter;
        private string Json { get { return stringWriter.ToString(); } }
        private readonly string lf = Environment.NewLine;

        [SetUp]
        public void Initialize()
        {
            stringWriter = new StringWriter();
            sut = new PrettyPrintingJsonStringWriter(stringWriter, "  ");
        }

        [Test]
        public void EmptyStructure()
        {
            sut.BeginStructure(null);
            sut.EndStructure();
            Json.ShouldBe("{ }");
        }

        [Test]
        public void EmptySequence()
        {
            sut.BeginSequence();
            sut.EndSequence();
            Json.ShouldBe("[ ]");
        }

        [Test]
        public void StructureWithSingleValueProperty()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.EndStructure();

            Json.ShouldBe(@"{" + lf
                        + @"  ""foo"": 1" + lf
                        + @"}");
        }

        [Test]
        public void StructureTwoValueProperties()
        {
            sut.BeginStructure(null);
            sut.AddProperty("foo");
            sut.Write(1);
            sut.AddProperty("bar");
            sut.Write(2);
            sut.EndStructure();

            Json.ShouldBe(@"{" + lf
                        + @"  ""foo"": 1," + lf
                        + @"  ""bar"": 2" + lf
                        + @"}");
        }

        [Test]
        public void SequenceWithSingleValue()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.EndSequence();

            Json.ShouldBe("[" + lf
                        + "  1" + lf
                        + "]");
        }

        [Test]
        public void SequenceWithTwoValues()
        {
            sut.BeginSequence();
            sut.Write(1);
            sut.Write(2);
            sut.EndSequence();

            Json.ShouldBe("[" + lf
                        + "  1," + lf
                        + "  2" + lf
                        + "]");
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
            sut.BeginStructure(null);
            sut.AddProperty("qux");
            sut.Write(2);
            sut.EndStructure();
            sut.EndStructure();

            Json.ShouldBe(@"{" + lf
                        + @"  ""foo"": {" + lf
                        + @"    ""bar"": 1" + lf
                        + @"  }," + lf
                        + @"  ""baz"": {" + lf
                        + @"    ""qux"": 2" + lf
                        + @"  }" + lf
                        + @"}");
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

            Json.ShouldBe("[" + lf
                        + "  [" + lf
                        + "    1" + lf
                        + "  ]," + lf
                        + "  [" + lf
                        + "    2" + lf
                        + "  ]" + lf
                        + "]");
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

            Json.ShouldBe("[" + lf
                        + "  { }," + lf
                        + "  { }" + lf
                        + "]");
        }

        [Test]
        public void Reference_WrittenOnOneLine()
        {
            sut.WriteReference(1);

            Json.ShouldBe(@"{ ""_ref"": 1 }");
        }

        [Test]
        public void ReferenceList()
        {
            sut.BeginSequence();
            sut.WriteReference(1);
            sut.WriteReference(2);
            sut.EndSequence();

            Json.ShouldBe(@"[" + lf
                        + @"  { ""_ref"": 1 }," + lf
                        + @"  { ""_ref"": 2 }" + lf
                        + @"]");
        }

        [Test]
        public void TypedStructure()
        {
            sut.BeginStructure("foo", null);
            sut.AddProperty("bar");
            sut.Write(1);
            sut.EndStructure();

            Json.ShouldBe(@"{" + lf
                        + @"  ""_type"": ""foo""," + lf
                        + @"  ""bar"": 1" + lf
                        + @"}");
        }
    }
}