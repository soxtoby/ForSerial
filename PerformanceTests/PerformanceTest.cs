using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using DAL;
using json.Json;
using json.Objects;
using json.PerformanceTests.Data;
using json.Tests;
using NUnit.Framework;
using ServiceStack.Text;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace json.PerformanceTests
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

        [Test]
        public void SerializeToNull()
        {
            DatabaseCompat db = GetNorthwindObject();
            Warmup(db);

            Stopwatch watch = Stopwatch.StartNew();
            SerializeManyTimes(db, new JsonStringWriter(TextWriter.Null));
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        [Test]
        public void XmlSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();

            XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCompat));
            serializer.Serialize(Stream.Null, db);

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
                serializer.Serialize(Stream.Null, db);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        [Test]
        public void ServiceStackSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();
            JsonSerializer<DatabaseCompat> serializer = new JsonSerializer<DatabaseCompat>();
            serializer.SerializeToWriter(db, TextWriter.Null);

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
                serializer.SerializeToWriter(db, TextWriter.Null);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private static void Warmup(DatabaseCompat db)
        {
            ObjectReader.Read(db, new JsonStringWriter(NullTextWriter.Instance));
        }

        private DatabaseCompat GetNorthwindObject()
        {
            string json = GetNorthwindJson();
            ObjectWriter<DatabaseCompat> objectWriter = new ObjectWriter<DatabaseCompat>();
            JsonParser.Parse(json, objectWriter);
            return objectWriter.Result;
        }

        private static void SerializeManyTimes(DatabaseCompat db, Writer writer)
        {
            for (int i = 0; i < 1000; i++)
                ObjectReader.Read(db, writer);
        }

        private string GetNorthwindJson()
        {
            Stream jsonStream = GetType().Assembly.GetManifestResourceStream("json.PerformanceTests.Data.northwind.json");
            byte[] buffer = new byte[jsonStream.Length];
            jsonStream.Read(buffer, 0, (int)jsonStream.Length);
            return Encoding.UTF8.GetString(buffer);
        }
    }

    internal class NullTextWriter : TextWriter
    {
        public static readonly NullTextWriter Instance = new NullTextWriter();

        private NullTextWriter() { }

        public override void Write(char value) { }

        public override void Write(double value) { }

        public override void Write(int value) { }

        public override void Write(string value) { }

        public override void WriteLine() { }

        public override Encoding Encoding { get { return Encoding.Default; } }
    }
}
