using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
            JsonReader.Read(originalJson, writer);

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

            Time(Iterations, () => ObjectReader.Read(db, new JsonStringWriter(TextWriter.Null)));
        }

        [Test]
        public void XmlSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();

            XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCompat));
            serializer.Serialize(Stream.Null, db);

            Time(Iterations, () => serializer.Serialize(Stream.Null, db));
        }

        [Test]
        public void ServiceStackSerialize()
        {
            DatabaseCompat db = GetNorthwindObject();
            JsonSerializer<DatabaseCompat> serializer = new JsonSerializer<DatabaseCompat>();
            serializer.SerializeToWriter(db, TextWriter.Null);

            Time(Iterations, () => serializer.SerializeToWriter(db, TextWriter.Null));
        }

        [Test]
        public void Deserialize()
        {
            GetNorthwindObject();
            string json = GetNorthwindJson();

            Time(100, () => JsonReader.Read(json, new ObjectWriter<DatabaseCompat>()));
        }

        [Test]
        public void ServiceStackDeserialize()
        {
            string json = GetNorthwindJson();
            json.FromJson<DatabaseCompat>();

            Time(100, () => json.FromJson<DatabaseCompat>());
        }

        [Test]
        public void XmlDeserialize()
        {
            DatabaseCompat db = GetNorthwindObject();
            XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCompat));
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, db);

            string xml = stringWriter.ToString();

            Time(100, () => new XmlSerializer(typeof(DatabaseCompat)).Deserialize(new StringReader(xml)));
        }

        [Test]
        public void Clone()
        {
            DatabaseCompat db = GetNorthwindObject();
            ObjectReader.Read(db, NullWriter.Instance);

            Time(100, () => ObjectReader.Read(db, new ObjectWriter<DatabaseCompat>()));
        }

        [Test]
        public void BinarySerializeClone()
        {
            DatabaseCompat db = GetNorthwindObject();

            Action clone = () =>
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, db);
                    ms.Position = 0;
                    bf.Deserialize(ms);
                };

            clone();

            Time(100, clone);
        }

        [Test]
        public void XmlSerializeClone()
        {
            DatabaseCompat db = GetNorthwindObject();

            Action clone = () =>
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DatabaseCompat));
                    StringWriter sw = new StringWriter();
                    serializer.Serialize(sw, db);
                    serializer.Deserialize(new StringReader(sw.ToString()));
                };

            Time(100, clone);
        }

        private static void Time(int iterations, Action action)
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
                action();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private DatabaseCompat GetNorthwindObject()
        {
            string json = GetNorthwindJson();
            ObjectWriter<DatabaseCompat> objectWriter = new ObjectWriter<DatabaseCompat>();
            JsonReader.Read(json, objectWriter);
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
