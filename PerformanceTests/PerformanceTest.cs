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
        private const int Iterations = 100;

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
            ObjectReader.Read(db, new JsonStringWriter(NullTextWriter.Instance));

            Time(() =>
                {
                    for (int i = 0; i < Iterations; i++)
                        ObjectReader.Read(db, new JsonStringWriter(TextWriter.Null));
                });
        }

        [Test]
        public void XmlSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();

            XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCompat));
            serializer.Serialize(Stream.Null, db);

            Time(() =>
                {
                    for (int i = 0; i < Iterations; i++)
                        serializer.Serialize(Stream.Null, db);
                });
        }

        [Test]
        public void ServiceStackSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();
            JsonSerializer<DatabaseCompat> serializer = new JsonSerializer<DatabaseCompat>();
            serializer.SerializeToWriter(db, TextWriter.Null);

            Time(() =>
                {
                    for (int i = 0; i < Iterations; i++)
                        serializer.SerializeToWriter(db, TextWriter.Null);
                });
        }

        private static void Time(Action action, string title = null)
        {
            Stopwatch watch = Stopwatch.StartNew();
            action();
            watch.Stop();

            if (title != null)
                Console.WriteLine(title + ": " + watch.ElapsedMilliseconds);
            else
                Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private DatabaseCompat GetNorthwindObject()
        {
            string json = GetNorthwindJson();
            ObjectWriter<DatabaseCompat> objectWriter = new ObjectWriter<DatabaseCompat>();
            JsonParser.Parse(json, objectWriter);
            return objectWriter.Result;
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
