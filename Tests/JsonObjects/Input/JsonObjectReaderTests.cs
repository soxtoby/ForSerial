using System.IO;
using EasyAssertions;
using ForSerial.Json;
using ForSerial.JsonObjects;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.JsonObjects
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

        private static string ConvertToJson(JsonObject jsonObject)
        {
            StringWriter stringWriter = new StringWriter();
            JsonStringWriter writer = new JsonStringWriter(stringWriter);
            JsonObjectReader.Read(jsonObject, writer);
            return stringWriter.ToString();
        }

        [Test]
        public void MaintainReferences()
        {
            Writer writer = Substitute.For<Writer>();
            JsonMap map = new JsonMap();
            map["foo"] = map["bar"] = new JsonMap();

            JsonObjectReader.Read(map, writer);

            writer.Received().WriteReference(1);
        }
    }
}