﻿using System.IO;
using ForSerial.Json;
using ForSerial.Objects;

namespace ForSerial
{
    public static class Transform
    {
        public static string ToJson(this object obj, ObjectParsingOptions options = null, string scenario = null)
        {
            using (SerializationScenario.Override(scenario ?? SerializationScenario.SerializeToJson))
            {
                StringWriter stringWriter = new StringWriter();
                ObjectReader.Read(obj, new JsonStringWriter(stringWriter), options);
                return stringWriter.ToString();
            }
        }

        public static string ToFormattedJson(this object obj, ObjectParsingOptions options = null, string scenario = null, string indentation = null)
        {
            using (SerializationScenario.Override(scenario ?? SerializationScenario.SerializeToJson))
            {
                StringWriter stringWriter = new StringWriter();
                ObjectReader.Read(obj, new PrettyPrintingJsonStringWriter(stringWriter, indentation), options);
                return stringWriter.ToString();
            }
        }

        public static string ToFormattedJson(this string json, string indentation = null)
        {
            StringWriter stringWriter = new StringWriter();
            JsonReader.Read(json, new PrettyPrintingJsonStringWriter(stringWriter, indentation));
            return stringWriter.ToString();
        }

        public static T ParseJson<T>(this string json, string scenario = null)
        {
            using (SerializationScenario.Override(scenario ?? SerializationScenario.DeserializeJson))
            {
                ObjectWriter<T> writer = new ObjectWriter<T>();
                JsonReader.Read(json, writer);
                return writer.Result;
            }
        }

        public static T CopyTo<T>(this object obj, ObjectParsingOptions options = null, string scenario = null)
        {
            using (SerializationScenario.Override(scenario ?? SerializationScenario.ObjectCopy))
            {
                ObjectWriter<T> writer = new ObjectWriter<T>();
                ObjectReader.Read(obj, writer, options);
                return writer.Result;
            }
        }
    }
}
