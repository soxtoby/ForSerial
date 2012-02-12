using System;
using json.Json;
using json.JsonObjects;
using NUnit.Framework;

namespace json.Tests.JsonObjects
{
    [TestFixture]
    public class JsonObjectReaderTests
    {
        [Test]
        public void EmptyObject()
        {
            Assert.AreEqual("{}", ConvertToJson(new JsonObject()));
        }

        [Test]
        public void NullProperty()
        {
            Assert.AreEqual(@"{""foo"":null}", ConvertToJson(new JsonObject { { "foo", null } }));
        }

        [Test]
        public void BooleanProperty()
        {
            Assert.AreEqual(@"{""foo"":true}", ConvertToJson(new JsonObject { { "foo", true } }));
            Assert.AreEqual(@"{""foo"":false}", ConvertToJson(new JsonObject { { "foo", false } }));
        }

        [Test]
        public void StringProperty()
        {
            Assert.AreEqual(@"{""foo"":""bar""}", ConvertToJson(new JsonObject { { "foo", "bar" } }));
        }

        [Test]
        public void NumberProperty()
        {
            Assert.AreEqual(@"{""foo"":5}", ConvertToJson(new JsonObject { { "foo", 5 } }));
        }

        [Test]
        public void EmptyObjectProperty()
        {
            Assert.AreEqual(@"{""foo"":{}}", ConvertToJson(new JsonObject { { "foo", new JsonObject() } }));
        }

        [Test]
        public void NonEmptyObjectProperty()
        {
            Assert.AreEqual(@"{""foo"":{""bar"":5}}", ConvertToJson(new JsonObject { { "foo", new JsonObject { { "bar", 5 } } } }));
        }

        [Test]
        public void NumberArrayProperty()
        {
            Assert.AreEqual(@"{""foo"":[1,2,3]}", ConvertToJson(new JsonObject { { "foo", new[] { 1, 2, 3 } } }));
        }

        [Test]
        public void MixedTypeArrayProperty()
        {
            Assert.AreEqual(@"{""foo"":[1,""two"",{""three"":4},[5]]}", ConvertToJson(
                new JsonObject
                    {
                        { "foo", new object[]
                            {
                                1,
                                "two",
                                new JsonObject { {"three", 4 }},
                                new[] { 5 },
                            }}
                    }));
        }

        [Test]
        public void ReadSubStructure()
        {
            JsonObject obj = new JsonObject { { "foo", new JsonObject { { "_type", "bar" }, { "baz", "qux" } } } };
            ReadSubStructureWriter valueFactory = new ReadSubStructureWriter();
            Convert.From.JsonObject(obj).WithBuilder(valueFactory);

            Assert.AreEqual(@"{""baz"":""qux""}", valueFactory.SubObjectJson);
        }

        private class ReadSubStructureWriter : TestWriter
        {
            public string SubObjectJson { get; set; }

            public override OutputStructure CreateStructure()
            {
                return new ReadSubStructureObject(this);
            }
        }

        private class ReadSubStructureObject : NullOutputStructure
        {
            private readonly ReadSubStructureWriter parentFactory;

            public ReadSubStructureObject(ReadSubStructureWriter parentFactory)
            {
                this.parentFactory = parentFactory;
            }

            public override bool SetType(string typeIdentifier, Reader reader)
            {
                Output jsonStringValue = reader.ReadSubStructure(JsonStringBuilder.Default);
                parentFactory.SubObjectJson = JsonStringBuilder.GetResult(jsonStringValue);
                return true;
            }
        }

        [Test]
        [ExpectedException(typeof(JsonObjectReader.InvalidObject))]
        public void InvalidObject()
        {
            ConvertToJson(new JsonObject { { "foo", new { } } });
        }

        [Test]
        [ExpectedException(typeof(JsonObjectReader.UnknownTypeCode))]
        public void UnknownTypeCode()
        {
            ConvertToJson(new JsonObject { { "foo", DBNull.Value } });
        }

        private static string ConvertToJson(JsonObject jsonObject)
        {
            return Convert.From.JsonObject(jsonObject).ToJson();
        }

        [Test]
        public void MaintainReferences()
        {
            var testBuilder = new WatchForReferenceBuilder();
            var jsonObject = new JsonObject();
            jsonObject["foo"] = jsonObject["bar"] = new JsonObject();
            Convert.From.JsonObject(jsonObject).WithBuilder(testBuilder);

            Assert.NotNull(testBuilder.ReferencedObject);
        }

        [Test]
        public void CreatePropertyObject()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.JsonObject(new JsonObject { { "foo", new JsonObject() } }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromProperties);
        }

        [Test]
        public void CreatePropertyArray()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.JsonObject(new JsonObject { { "foo", new object[] { } } }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromProperties);
        }

        [Test]
        public void CreateArrayObject()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.JsonObject(new JsonObject { { "foo", new[] { new JsonObject() } } }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ObjectsCreatedFromArrays);
        }

        [Test]
        public void CreateArrayArray()
        {
            var valueFactory = new CustomCreateWriter();
            Convert.From.JsonObject(new JsonObject { { "foo", new[] { new object[] { } } } }).WithBuilder(valueFactory);

            Assert.AreEqual(1, valueFactory.ArraysCreatedFromArrays);
        }
    }
}