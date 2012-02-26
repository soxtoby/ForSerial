using System.IO;
using json.Json;
using json.JsonObjects;
using NUnit.Framework;

namespace json.Tests.JsonObjects
{
    [TestFixture]
    public class JsonObjectReaderTests
    {
        [Test]
        public void Value()
        {
            ConvertToJson(new JsonValue(1))
                .ShouldBe("1");
        }

        [Test]
        public void EmptyArray()
        {
            ConvertToJson(new JsonArray())
                .ShouldBe("[]");
        }

        [Test]
        public void NumberArray()
        {
            ConvertToJson(new JsonArray { 1, 2, 3 })
                .ShouldBe("[1,2,3]");
        }

        [Test]
        public void MixedTypeArray()
        {
            ConvertToJson(new JsonArray
                {
                    1,
                    "two",
                    new JsonMap { { "three", 4 } },
                    new JsonArray { 5 },
                })
                .ShouldBe(@"[1,""two"",{""three"":4},[5]]");
        }

        [Test]
        public void EmptyObject()
        {
            ConvertToJson(new JsonMap())
                .ShouldBe("{}");
        }

        [Test]
        public void ValueProperty()
        {
            ConvertToJson(new JsonMap { { "foo", 1 } })
                .ShouldBe(@"{""foo"":1}");
        }

        [Test]
        public void EmptyObjectProperty()
        {
            ConvertToJson(new JsonMap { { "foo", new JsonMap() } })
                .ShouldBe(@"{""foo"":{}}");
        }

        [Test]
        public void NonEmptyObjectProperty()
        {
            ConvertToJson(new JsonMap { { "foo", new JsonMap { { "bar", 5 } } } })
                .ShouldBe(@"{""foo"":{""bar"":5}}");
        }

        [Test]
        public void EmptyArrayProperty()
        {
            ConvertToJson(new JsonMap { { "foo", new JsonArray() } })
                .ShouldBe(@"{""foo"":[]}");
        }

        [Test]
        public void NumberArrayProperty()
        {
            ConvertToJson(new JsonMap { { "foo", new JsonArray { 1, 2, 3 } } })
                .ShouldBe(@"{""foo"":[1,2,3]}");
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            ConvertToJson(new JsonMap
                {
                    { "foo", new JsonArray
                        {
                            1,
                            "two",
                            new JsonMap { {"three", 4 }},
                            new JsonArray { 5 },
                        }
                    }
                })
                .ShouldBe(@"{""foo"":[1,""two"",{""three"":4},[5]]}");
        }

        //[Test]    // TODO reimplement ReadSubStructure
        //public void ReadSubStructure()
        //{
        //    JsonMap obj = new JsonMap { { "foo", new JsonMap { { "_type", "bar" }, { "baz", "qux" } } } };
        //    ReadSubStructureWriter valueFactory = new ReadSubStructureWriter();
        //    Convert.From.JsonObject(obj).WithBuilder(valueFactory);

        //    Assert.AreEqual(@"{""baz"":""qux""}", valueFactory.SubObjectJson);
        //}

        //private class ReadSubStructureWriter : TestWriter
        //{
        //    public string SubObjectJson { get; set; }

        //    public override OutputStructure BeginStructure()
        //    {
        //        return new ReadSubStructureObject(this);
        //    }
        //}

        //private class ReadSubStructureObject : NullOutputStructure
        //{
        //    private readonly ReadSubStructureWriter parentFactory;

        //    public ReadSubStructureObject(ReadSubStructureWriter parentFactory)
        //    {
        //        this.parentFactory = parentFactory;
        //    }

        //    public override bool SetType(string typeIdentifier, Reader reader)
        //    {
        //        //Output jsonStringValue = reader.ReadSubStructure(JsonStringBuilder.GetDefault());
        //        //parentFactory.SubObjectJson = JsonStringBuilder.GetResult(jsonStringValue);
        //        return true;
        //    }
        //}

        private static string ConvertToJson(JObject jsonObject)
        {
            StringWriter stringWriter = new StringWriter();
            JsonStringWriter writer = new JsonStringWriter(stringWriter);
            JsonObjectReader.Read(jsonObject, writer);
            return stringWriter.ToString();
        }

        //[Test]    // TODO reimplement maintaining references
        //public void MaintainReferences()
        //{
        //    var testBuilder = new WatchForReferenceBuilder();
        //    var jsonObject = new JsonMap();
        //    jsonObject["foo"] = jsonObject["bar"] = new JsonMap();
        //    Convert.From.JsonObject(jsonObject).WithBuilder(testBuilder);

        //    Assert.NotNull(testBuilder.ReferencedObject);
        //}

        // TODO Remove if there's no more property context stuff
        //[Test]
        //public void CreatePropertyObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.JsonObject(new JsonMap { { "foo", new JsonMap() } }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        //}

        //[Test]
        //public void CreatePropertyArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.JsonObject(new JsonMap { { "foo", new object[] { } } }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        //}

        //[Test]
        //public void CreateArrayObject()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.JsonObject(new JsonMap { { "foo", new[] { new JsonMap() } } }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        //}

        //[Test]
        //public void CreateArrayArray()
        //{
        //    var valueFactory = new CustomCreateWriter();
        //    Convert.From.JsonObject(new JsonMap { { "foo", new[] { new object[] { } } } }).WithBuilder(valueFactory);

        //    Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        //}
    }
}