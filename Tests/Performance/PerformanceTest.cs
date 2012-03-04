using System.IO;
using System.Text;
using DAL;
using json.Json;
using json.Objects;
using NUnit.Framework;

namespace json.Tests.Performance
{
    [TestFixture]
    public class PerformanceTest
    {
        [Test]
        public void VerifyNorthwindDeserializeReserialize()
        {
            string originalJson = GetNorthwindJson();
            ObjectWriter<Database> writer = new ObjectWriter<Database>();
            JsonParser.Parse(originalJson, writer);

            Database db = writer.Result;

            db.ShouldNotBeNull();

            StringWriter stringWriter = new StringWriter();
            PrettyPrintingJsonStringWriter jsonWriter = new PrettyPrintingJsonStringWriter(stringWriter);
            ObjectReader.Read(db, jsonWriter, new ObjectParsingOptions { SerializeTypeInformation = TypeInformationLevel.None });

            string result = stringWriter.ToString();

            result.ShouldBe(originalJson);
        }

        private string GetNorthwindJson()
        {
            Stream jsonStream = GetType().Assembly.GetManifestResourceStream("json.Tests.Performance.northwind.json");
            byte[] buffer = new byte[jsonStream.Length];
            jsonStream.Read(buffer, 0, (int)jsonStream.Length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
